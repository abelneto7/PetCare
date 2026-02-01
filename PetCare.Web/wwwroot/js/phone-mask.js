(function () {
    function onlyDigits(value) {
        return (value || "").replace(/\D/g, "");
    }

    function formatBRPhone(digits) {
        if (!digits) return "";

        digits = digits.substring(0, 11);

        const ddd = digits.substring(0, 2);
        const rest = digits.substring(2);

        if (digits.length <= 2) return `(${ddd}`;

        if (rest.length <= 4) return `(${ddd}) ${rest}`;

        if (rest.length <= 8) {
            return `(${ddd}) ${rest.substring(0, 4)}-${rest.substring(4)}`;
        }

        return `(${ddd}) ${rest.substring(0, 5)}-${rest.substring(5)}`;
    }

    function applyMask(input) {
        const digits = onlyDigits(input.value);
        input.value = formatBRPhone(digits);
    }

    document.addEventListener("input", function (e) {
        const el = e.target;
        if (el && el.matches && el.matches("[data-phone-mask]")) {
            applyMask(el);
        }
    });

    document.addEventListener("blur", function (e) {
        const el = e.target;
        if (el && el.matches && el.matches("[data-phone-mask]")) {
            applyMask(el);
        }
    }, true);

    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll("[data-phone-mask]").forEach(applyMask);
    });
})();
