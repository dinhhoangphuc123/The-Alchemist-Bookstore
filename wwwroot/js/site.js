(function () {
  // ---- Header co lại + đổ bóng khi cuộn ----
  var header = document.getElementById('main-header');
  var container = document.getElementById('header-container');
  function onScroll() {
    var scrolled = window.scrollY > 40;
    header.classList.toggle('shadow-sm', scrolled);
    container.classList.toggle('h-16', scrolled);
    container.classList.toggle('h-20', !scrolled);
  }
  if (header && container) {
    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
  }

  // ---- Menu mobile ----
  var toggle = document.getElementById('mobile-toggle');
  var menu = document.getElementById('mobile-menu');
  if (toggle && menu) {
    toggle.addEventListener('click', function () { menu.classList.toggle('hidden'); });
  }

  // ---- Ẩn/hiện khối bất kỳ: <button data-toggle="#selector"> ----
  document.querySelectorAll('[data-toggle]').forEach(function (btn) {
    btn.addEventListener('click', function () {
      var target = document.querySelector(btn.getAttribute('data-toggle'));
      if (!target) return;
      target.classList.toggle('hidden');
      if (!target.classList.contains('hidden')) {
        target.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
      }
    });
  });

  // ---- Đóng modal: <button data-close="#selector"> ----
  document.querySelectorAll('[data-close]').forEach(function (btn) {
    btn.addEventListener('click', function () {
      var target = document.querySelector(btn.getAttribute('data-close'));
      if (target) target.remove();
    });
  });

  // ---- Vòng tròn quay cho MỌI thao tác chuyển trang (liên kết + gửi form) ----
  var overlay = document.getElementById('loading-overlay');
  var overlayText = document.getElementById('loading-text');
  var busy = false;                              // đang chuyển trang -> chặn bấm lặp

  function showLoading(text) {
    if (!overlay) return;
    overlayText.textContent = text || 'Đang tải trang...';
    overlay.classList.remove('hidden');
    overlay.classList.add('flex');
  }
  function hideLoading() {
    if (!overlay) return;
    overlay.classList.add('hidden');
    overlay.classList.remove('flex');
    busy = false;
  }

  // Liên kết nội bộ (menu, nút "Đặt sách", "Mua lại", tab lọc đơn, footer...)
  document.addEventListener('click', function (e) {
    if (e.defaultPrevented || e.button !== 0 || e.ctrlKey || e.metaKey || e.shiftKey || e.altKey) return;
    var a = e.target.closest ? e.target.closest('a[href]') : null;
    if (!a || a.hasAttribute('data-no-loading') || a.hasAttribute('download')) return;
    if (a.target && a.target !== '_self') return;

    var href = a.getAttribute('href') || '';
    if (href.charAt(0) === '#' || /^(mailto:|tel:|javascript:)/i.test(href)) return;
    if (a.origin !== window.location.origin) return;
    if (a.pathname === window.location.pathname && a.search === window.location.search && a.hash) return; // chỉ nhảy neo trong trang

    e.preventDefault();
    if (busy) return;
    busy = true;
    showLoading(a.getAttribute('data-loading') || 'Đang tải trang...');
    setTimeout(function () { window.location.href = a.href; }, 500);
  });

  // Mọi form (đăng nhập/đăng ký/đăng xuất, đặt sách, gửi đánh giá, cập nhật trạng thái đơn...)
  document.addEventListener('submit', function (e) {
    var form = e.target;
    if (!form || form.tagName !== 'FORM' || form.hasAttribute('data-no-loading')) return;
    if (e.defaultPrevented) return;              // vd. confirm() bị người dùng bấm Hủy

    e.preventDefault();                           // sự kiện này chỉ chạy khi form đã qua kiểm tra HTML5
    if (busy) return;
    busy = true;

    var text = form.getAttribute('data-loading');
    if (!text) {
      var action = form.getAttribute('action') || '';
      if (action.indexOf('/dat-sach') !== -1) text = 'Đang đặt sách...';
      else if (action.indexOf('/danh-gia') !== -1) text = 'Đang gửi đánh giá...';
      else if (action.indexOf('/trang-thai') !== -1) text = 'Đang cập nhật đơn hàng...';
      else text = 'Đang xử lý...';
    }
    showLoading(text);
    setTimeout(function () { form.submit(); }, 700);   // cho người dùng kịp thấy vòng quay
  });

  // Bấm nút Back của trình duyệt: tắt overlay nếu trang được lấy lại từ cache
  window.addEventListener('pageshow', function (e) {
    if (e.persisted) hideLoading();
  });

  // ---- Ô số điện thoại: chỉ nhận chữ số, tối đa 10 ----
  document.querySelectorAll('input[data-digits]').forEach(function (input) {
    input.addEventListener('input', function () {
      input.value = input.value.replace(/\D/g, '').slice(0, 10);
    });
  });

  // ---- Bộ chọn số lượng + tổng tiền ----
  var summary = document.getElementById('order-summary');
  if (summary) {
    var unit = Number(summary.getAttribute('data-unit-price'));
    var input = document.querySelector('[data-qty-input]');
    var display = document.getElementById('qty-display');
    var total = document.getElementById('total-price');
    var qty = Number(input.value) || 1;

    function render() {
      input.value = qty;
      display.textContent = qty;
      total.textContent = (qty * unit).toLocaleString('vi-VN') + ' ₫';
    }

    summary.querySelectorAll('[data-qty]').forEach(function (btn) {
      btn.addEventListener('click', function () {
        qty = Math.min(50, Math.max(1, qty + Number(btn.getAttribute('data-qty'))));
        render();
      });
    });
    render();
  }
})();
