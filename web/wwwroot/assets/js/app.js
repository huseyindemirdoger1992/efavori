/* ============================================================================
   efavori.com — app.js (Dashtreme tema scripti, hardened)
   ----------------------------------------------------------------------------
   ORİJİNAL DOSYADAKİ HATA:
   Tüm dosya tek bir $(function(){ ... }) içinde VİRGÜLLE ZİNCİRLENMİŞ tek bir
   ifadeydi. Zincirin ilk adımı olan `new PerfectScrollbar(".app-container")`,
   element DOM'da yoksa `throw` eder. Tek ifade olduğu için istisna fırladığı
   anda zincirin geri kalanı — mm-active döngüsü ve $("#menu").metisMenu()
   dahil — HİÇ çalışmaz. Sonuç: sidebar alt menüleri mm-collapse sınıfını
   alamaz, hepsi açık doğar ve tıklama ile kapanmaz.

   DÜZELTMELER:
   1. PerfectScrollbar çağrıları element varlığı kontrolüyle guard'landı.
   2. Virgül zinciri bağımsız ifadelere bölündü (bir hata diğerlerini öldürmez).
   3. metisMenu init'i tek-sefer garantili hale getirildi.
   4. Aktif menü tespiti `this.href == window.location` tam-eşitliğinden
      çıkarıldı; sondaki "/", culture segmenti ve query string'e dayanıklı,
      "en uzun önek" mantığıyla TEK dal işaretleyen sürümle değiştirildi.
   ========================================================================== */

$(function () {
    "use strict";

    /* ------------------------------------------------------------------
       1) PERFECT SCROLLBAR — element yoksa sessizce atla
       ------------------------------------------------------------------ */
    function initScrollbar(selector) {
        if (typeof PerfectScrollbar !== "function") return;
        var el = document.querySelector(selector);
        if (!el) return;                       // ← eski sürümde burada throw ediyordu
        try { new PerfectScrollbar(selector); } catch (e) { }
    }

    initScrollbar(".app-container");
    initScrollbar(".header-message-list");
    initScrollbar(".header-notifications-list");

    /* ------------------------------------------------------------------
       2) ARAMA ÇUBUĞU
       ------------------------------------------------------------------ */
    $(".mobile-search-icon").on("click", function () {
        $(".search-bar").addClass("full-search-bar");
    });

    $(".search-close").on("click", function () {
        $(".search-bar").removeClass("full-search-bar");
    });

    /* ------------------------------------------------------------------
       3) SIDEBAR AÇ / KAPA
       ------------------------------------------------------------------ */
    $(".mobile-toggle-menu").on("click", function () {
        $(".wrapper").addClass("toggled");
    });

    $(".toggle-icon").on("click", function () {
        if ($(".wrapper").hasClass("toggled")) {
            $(".wrapper").removeClass("toggled");
            $(".sidebar-wrapper").off("mouseenter mouseleave");
        } else {
            $(".wrapper").addClass("toggled");
            $(".sidebar-wrapper").hover(
                function () { $(".wrapper").addClass("sidebar-hovered"); },
                function () { $(".wrapper").removeClass("sidebar-hovered"); }
            );
        }
    });

    /* ------------------------------------------------------------------
       4) SCROLL DAVRANIŞLARI
       ------------------------------------------------------------------ */
    $(window).on("scroll", function () {
        var top = $(this).scrollTop();

        if (top > 300) { $(".back-to-top").fadeIn(); }
        else { $(".back-to-top").fadeOut(); }

        if (top > 60) { $(".topbar").addClass("bg-dark"); }
        else { $(".topbar").removeClass("bg-dark"); }
    });

    $(".back-to-top").on("click", function () {
        $("html, body").animate({ scrollTop: 0 }, 600);
        return false;
    });

    /* ------------------------------------------------------------------
       5) AKTİF MENÜ TESPİTİ
       ------------------------------------------------------------------
       Eski sürüm: this.href == window.location  (tam string eşitliği)
       Bu projede tutmuyor çünkü:
         - menü linklerinin 222 tanesi "/" ile bitiyor, route'lar bitmiyor
         - link culture'ı (CurrentCulture) sayfa culture'ından farklı olabiliyor
         - query string / hash eşleşmeyi bozuyor
         - 7 URL menüde iki yerde geçiyor → iki dal birden açılıyordu
       Yeni sürüm: normalize + culture'dan bağımsız + en uzun önek = TEK dal.
       ------------------------------------------------------------------ */
    function markActiveMenu() {
        var menu = document.getElementById("menu");
        if (!menu) return;

        // SideBarMenu.razor içindeki inline script zaten işaretlediyse tekrar etme
        if (menu.querySelector("li.mm-active")) return;

        var CULTURES = ["tr", "en", "az", "de", "es", "fr", "hi", "pt", "ru", "zh"];

        function normalize(path) {
            path = (path || "").split("?")[0].split("#")[0];
            try { path = decodeURIComponent(path); } catch (e) { }
            path = path.toLowerCase().replace(/\/+$/, "");
            var parts = path.split("/");
            if (parts.length > 1 && CULTURES.indexOf(parts[1]) > -1) parts.splice(1, 1);
            path = parts.join("/");
            return path === "" ? "/" : path;
        }

        var here = normalize(location.pathname);
        var best = null, bestLen = -1;
        var links = menu.querySelectorAll("a[href]");

        for (var i = 0; i < links.length; i++) {
            var raw = (links[i].getAttribute("href") || "").toLowerCase();
            if (raw === "" || raw.charAt(0) === "#" || raw.indexOf("javascript:") === 0) continue;

            var target = normalize(links[i].pathname);
            if (target === "/") continue;

            if (here === target || here.indexOf(target + "/") === 0) {
                if (target.length > bestLen) { bestLen = target.length; best = links[i]; }
            }
        }

        if (!best) return;

        best.classList.add("active");
        var node = best.parentNode;
        while (node && node !== menu.parentNode) {
            if (node.tagName === "LI") node.classList.add("mm-active");
            else if (node.tagName === "UL" && node !== menu) node.classList.add("mm-show");
            node = node.parentNode;
        }
    }

    /* ------------------------------------------------------------------
       6) METISMENU — mm-active işaretlendikten SONRA, tek sefer
       ------------------------------------------------------------------
       Sıra kritik: metisMenu init'i mevcut li.mm-active dallarının <ul>'una
       "mm-collapse mm-show", diğerlerine "mm-collapse" ekler. mm-active
       önceden işaretlenmezse doğru dal açık başlamaz.
       ------------------------------------------------------------------ */
    markActiveMenu();

    var $menu = $("#menu");
    if ($menu.length && $.fn.metisMenu && !$menu.data("mm-initialized")) {
        $menu.data("mm-initialized", true);
        $menu.metisMenu();
    }

    /* ------------------------------------------------------------------
       7) CHAT / E-POSTA PANELLERİ
       ------------------------------------------------------------------ */
    $(".chat-toggle-btn").on("click", function () {
        $(".chat-wrapper").toggleClass("chat-toggled");
    });
    $(".chat-toggle-btn-mobile").on("click", function () {
        $(".chat-wrapper").removeClass("chat-toggled");
    });
    $(".email-toggle-btn").on("click", function () {
        $(".email-wrapper").toggleClass("email-toggled");
    });
    $(".email-toggle-btn-mobile").on("click", function () {
        $(".email-wrapper").removeClass("email-toggled");
    });
    $(".compose-mail-btn").on("click", function () {
        $(".compose-mail-popup").show();
    });
    $(".compose-mail-close").on("click", function () {
        $(".compose-mail-popup").hide();
    });

    /* ------------------------------------------------------------------
       8) TEMA SEÇİCİ
       ------------------------------------------------------------------ */
    $(".switcher-btn").on("click", function () {
        $(".switcher-wrapper").toggleClass("switcher-toggled");
    });
    $(".close-switcher").on("click", function () {
        $(".switcher-wrapper").removeClass("switcher-toggled");
    });

    for (var t = 1; t <= 15; t++) {
        (function (index) {
            $("#theme" + index).on("click", function () {
                $("body").attr("class", "bg-theme bg-theme" + index);
            });
        })(t);
    }
});