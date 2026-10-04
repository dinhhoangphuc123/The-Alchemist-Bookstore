-- =========================================================
-- PHẦN 1: TẠO BẢNG (giữ nguyên cấu trúc script gốc của bạn,
-- chỉ thêm IF NOT EXISTS để chạy lại không báo lỗi)
-- =========================================================

CREATE TABLE IF NOT EXISTS books (
    book_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    category VARCHAR(100),
    title VARCHAR(255),
    subtitle TEXT,
    author VARCHAR(255) NOT NULL,
    price NUMERIC(12, 2) NOT NULL CHECK (price >= 0),
    promotion NUMERIC(5, 2) DEFAULT 0
        CHECK (promotion >= 0 AND promotion <= 100),
    image VARCHAR(500),
    description TEXT,
    published_at TIMESTAMP,
    press TEXT,
    file_remark TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS order_status (
    status_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    status_name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS orders (
    order_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    order_code VARCHAR(50) NOT NULL UNIQUE,
    quantity INT NOT NULL CHECK (quantity > 0),
    customer_name VARCHAR(255) NOT NULL,
    phone VARCHAR(20) NOT NULL,
    payment_method VARCHAR(50) NOT NULL,
    order_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    total_amount NUMERIC(12, 2) NOT NULL CHECK (total_amount >= 0),
    status_id BIGINT NOT NULL,
    CONSTRAINT fk_orders_status
        FOREIGN KEY (status_id)
        REFERENCES order_status(status_id)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS admin (
    admin_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    full_name VARCHAR(255),
    email VARCHAR(255) UNIQUE,
    role VARCHAR(50) NOT NULL DEFAULT 'admin',
    status BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS feedback (
    feedback_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    book_id BIGINT NOT NULL,
    customer_name VARCHAR(255) NOT NULL,
    rating SMALLINT NOT NULL CHECK (rating >= 1 AND rating <= 5),
    content TEXT,
    status VARCHAR(50) NOT NULL DEFAULT 'pending'
        CHECK (status IN ('pending', 'approved', 'rejected')),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_feedback_book
        FOREIGN KEY (book_id)
        REFERENCES books(book_id)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS settings (
    setting_id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    setting_key VARCHAR(100) NOT NULL UNIQUE,
    setting_value TEXT,
    description TEXT,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_feedback_book_id ON feedback(book_id);
CREATE INDEX IF NOT EXISTS idx_feedback_rating  ON feedback(rating);
CREATE INDEX IF NOT EXISTS idx_orders_status_id ON orders(status_id);
CREATE INDEX IF NOT EXISTS idx_orders_order_time ON orders(order_time);
CREATE INDEX IF NOT EXISTS idx_orders_phone     ON orders(phone);

-- =========================================================
-- PHẦN 2: BỔ SUNG CỘT + RÀNG BUỘC + DỮ LIỆU MẪU
-- Chạy được nhiều lần (idempotent).
-- Lưu ý: nếu bảng orders đã có dữ liệu, hãy xóa trước (các cột mới là NOT NULL).
-- =========================================================

ALTER TABLE orders   ADD COLUMN IF NOT EXISTS book_id    BIGINT       NOT NULL
    REFERENCES books(book_id) ON UPDATE CASCADE ON DELETE RESTRICT;
ALTER TABLE orders   ADD COLUMN IF NOT EXISTS account_id BIGINT       NOT NULL
    REFERENCES admin(admin_id) ON UPDATE CASCADE ON DELETE RESTRICT;
ALTER TABLE orders   ADD COLUMN IF NOT EXISTS address    VARCHAR(500) NOT NULL;
ALTER TABLE orders   ADD COLUMN IF NOT EXISTS note       VARCHAR(500);
ALTER TABLE feedback ADD COLUMN IF NOT EXISTS badge      VARCHAR(100);

CREATE INDEX IF NOT EXISTS idx_orders_account_id ON orders(account_id);

-- 2. RÀNG BUỘC TOÀN VẸN Ở TẦNG DB ---------------------------------------------
-- SĐT: đúng 10 chữ số, bắt đầu bằng 0
ALTER TABLE orders DROP CONSTRAINT IF EXISTS chk_orders_phone;
ALTER TABLE orders ADD  CONSTRAINT chk_orders_phone CHECK (phone ~ '^0[0-9]{9}$');

ALTER TABLE orders DROP CONSTRAINT IF EXISTS chk_orders_payment;
ALTER TABLE orders ADD  CONSTRAINT chk_orders_payment CHECK (payment_method IN ('COD', 'Banking'));

ALTER TABLE admin DROP CONSTRAINT IF EXISTS chk_admin_role;
ALTER TABLE admin ADD  CONSTRAINT chk_admin_role CHECK (role IN ('admin', 'customer'));

-- 3. DỮ LIỆU MẪU --------------------------------------------------------------
INSERT INTO order_status (status_name, description) VALUES
    ('Chờ xác nhận',   'Đơn mới, đang chờ cửa hàng xác nhận'),
    ('Đã xác nhận',    'Cửa hàng đã xác nhận, đang chuẩn bị hàng'),
    ('Đang giao hàng', 'Đơn đã giao cho đơn vị vận chuyển'),
    ('Hoàn thành',     'Khách đã nhận hàng'),
    ('Đã hủy',         'Đơn đã bị hủy')
ON CONFLICT (status_name) DO NOTHING;

-- Quy ước cột: title = tiêu đề lớn, subtitle = câu trích dẫn ở hero,
-- description = đoạn giới thiệu, file_remark = ghi chú ấn bản (hiện cạnh năm xuất bản)
INSERT INTO books (name, category, title, subtitle, author, price, promotion, image,
                   description, published_at, press, file_remark)
SELECT 'Nhà Giả Kim (The Alchemist)',
       'Tiểu thuyết triết lý, Tự lực, Văn học kinh điển',
       'Nhà Giả Kim',
       '“Khi bạn thực sự mong muốn một điều gì, cả vũ trụ sẽ hợp lực giúp bạn đạt được điều đó.”',
       'Paulo Coelho',
       169000, 23.67,   -- giá bán = 169.000 × (1 − 23,67%) ≈ 129.000 ₫ (làm tròn nghìn)
       'https://lh3.googleusercontent.com/aida-public/AB6AXuAombiIoPiFHwQUKgiEGs8slZsm5aNrO73k3Nnj0Pp3Wdc9IycMaiNk7iHW73TZ90w8thde0Q2MAsxxa5psHytgbJmVi1CwFNvHEC-1GDJUgS5WD7FEtGT1kewW5Hzj7pS-ifhZDcopiE7P2J-sPcqCsIhCYprsGHSq5QucJI-zRpn83GO0-yAkcz8RhKOD0u2-Nu1Fv91JlslpFAQNZNy0EJCqloU8Dzsebi56KqTftCVLn28BI9GG',
       'Kiệt tác của Paulo Coelho kể về hành trình của Santiago qua sa mạc Bắc Phi để tìm kiếm kho báu và khám phá ý nghĩa của ước mơ, niềm tin và hành trình của chính mình.',
       '2026-01-01', 'NXB Văn Học', 'Tái bản ấn phẩm đặc biệt'
WHERE NOT EXISTS (SELECT 1 FROM books);

INSERT INTO settings (setting_key, setting_value, description) VALUES
    ('banner_image',  'https://lh3.googleusercontent.com/aida-public/AB6AXuDtiY9eEQE9F_mIz8O7UPA5EFup5orFCrw0ImNGmqc9_4VDSFObU3HGDI3ty_LMkW9gN_9hCBfH5YB3x5YH42RfC3_ouMapexvQTDLxPyy1r7vD9Csa98mUDM3A7mJKUphvu7YkRqX7oaWpOgnQh4yP_6JhJ46MQGpNRBMKN854aB2Rh4AMFZEUv58DT8vBeR_U_X9lpQjx25z4IJ641Z_iSR49zoAZGmLQctSMubkK7Xq42vtZedmy', 'Ảnh banner sa mạc (trang Nội dung)'),
    ('author_image',  'https://lh3.googleusercontent.com/aida-public/AB6AXuA7WQeCYJOPHebfAo5-Pu_AnGW7UQcA7p8IKe_5QBaT38C3L2qvilx9830s6uBqZ41vbURHgVHQXFKuulVLadWmUXIXSu8dDNBB3ICfXxPj3IPeX50fsySID1DoFxPtjie3mUTyH9-8gJN_-x-4EH0nT7UTS_4pc3RdDFd6UFS2Rg-hrxfnRmybVCANg6enB9ilmQpOAfgeQJ0CmKXHPWAEjnQNyEt1DYqQscD4tZFGOVinHO9UKB-5', 'Chân dung tác giả (trang Tác giả)'),
    ('book_language', 'Tiếng Việt (Bản dịch chuẩn)', 'Ngôn ngữ (trang Thông tin)'),
    ('book_cover',    'Bìa cứng cao cấp (Hardcover)', 'Định dạng bìa (trang Thông tin)'),
    ('book_pages',    '228 trang', 'Số trang (trang Thông tin)'),
    ('shipping_info', 'Toàn quốc trong 2-3 ngày', 'Thời gian giao hàng (trang Giới thiệu)'),
    ('site_email',    'contact@nhagiakim.vn', 'Email liên hệ (footer)'),
    ('site_hotline',  '1900 6868 (8:00 - 20:00)', 'Hotline (footer)'),
    ('site_address',  'Hà Nội & TP. Hồ Chí Minh, Việt Nam', 'Địa chỉ (footer)')
ON CONFLICT (setting_key) DO NOTHING;

INSERT INTO feedback (book_id, customer_name, rating, content, status, badge)
SELECT b.book_id, v.n, v.r, v.c, 'approved', v.bd
FROM (SELECT book_id FROM books ORDER BY book_id LIMIT 1) b
CROSS JOIN (VALUES
    ('Nguyễn Minh', 5,
     '“Một cuốn sách khiến mình suy nghĩ nhiều hơn về mục tiêu và hành trình của bản thân. Mỗi độ tuổi đọc lại đều mang đến những chiêm nghiệm hoàn toàn khác biệt.”',
     'Đã mua bản bìa cứng'),
    ('Trần Hoàng Long', 5,
     '“Thiết kế sách đen trắng tối giản lần này rất đẹp và cầm chắc tay. Nội dung vẫn luôn là một kiệt tác thúc đẩy con người hành động và vượt qua nỗi sợ thất bại.”',
     'Đã mua tại website'),
    ('Lê Thu Trang', 5,
     '“Đoạn hội thoại giữa Santiago và Nhà Giả Kim giữa sa mạc luôn là phần mình xúc động nhất. Cuốn sách gối đầu giường của bất cứ ai đang mất phương hướng.”',
     'Độc giả trung thành')
) AS v(n, r, c, bd)
WHERE NOT EXISTS (SELECT 1 FROM feedback);

-- Tài khoản admin / khachhang được tạo tự động khi chạy web lần đầu (mật khẩu được băm bằng C#).





