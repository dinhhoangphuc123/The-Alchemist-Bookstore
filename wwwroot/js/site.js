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
