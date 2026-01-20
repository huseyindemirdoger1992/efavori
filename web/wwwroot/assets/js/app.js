$(function () {
    "use strict";

    // Kaydýrma çubuklarýný (Scrollbar) baþlat
    new PerfectScrollbar(".app-container");
    new PerfectScrollbar(".header-message-list");
    new PerfectScrollbar(".header-notifications-list");

    // Mobil Arama Açma
    $(".mobile-search-icon").on("click", function () {
        $(".search-bar").addClass("full-search-bar");
    });

    // Mobil Arama Kapatma
    $(".search-close").on("click", function () {
        $(".search-bar").removeClass("full-search-bar");
    });

    // Mobil Menü Toggle
    $(".mobile-toggle-menu").on("click", function () {
        $(".wrapper").addClass("toggled");
    });

    // Sidebar Toggle ve Hover Durumu
    $(".toggle-icon").click(function () {
        if ($(".wrapper").hasClass("toggled")) {
            $(".wrapper").removeClass("toggled");
            $(".sidebar-wrapper").unbind("hover");
        } else {
            $(".wrapper").addClass("toggled");
            $(".sidebar-wrapper").hover(
                function () {
                    $(".wrapper").addClass("sidebar-hovered");
                },
                function () {
                    $(".wrapper").removeClass("sidebar-hovered");
                }
            );
        }
    });

    // Sayfa Kaydýrma Ýþlemleri (Back to Top ve Topbar Renk Deðiþimi)
    $(window).on("scroll", function () {
        // Yukarý çýk butonu görünürlüðü
        if ($(this).scrollTop() > 300) {
            $(".back-to-top").fadeIn();
        } else {
            $(".back-to-top").fadeOut();
        }

        // Topbar arka plan deðiþimi
        if ($(this).scrollTop() > 60) {
            $('.topbar').addClass('bg-dark');
        } else {
            $('.topbar').removeClass('bg-dark');
        }
    });

    // Yukarý çýk butonu týklama animasyonu
    $(".back-to-top").on("click", function () {
        $("html, body").animate({
            scrollTop: 0
        }, 600);
        return false;
    });

    // Aktif Menü Linkini Belirleme (Auto-active)
    var currentUrl = window.location.href;
    $(".metismenu li a").filter(function () {
        return this.href == currentUrl;
    }).addClass("").parent().addClass("mm-active")
        .parents("ul").addClass("mm-show")
        .parent("li").addClass("mm-active");

    // MetisMenu Baþlatma
    $("#menu").metisMenu();

    // Chat Paneli Toggle
    $(".chat-toggle-btn").on("click", function () {
        $(".chat-wrapper").toggleClass("chat-toggled");
    });
    $(".chat-toggle-btn-mobile").on("click", function () {
        $(".chat-wrapper").removeClass("chat-toggled");
    });

    // Email Paneli Toggle
    $(".email-toggle-btn").on("click", function () {
        $(".email-wrapper").toggleClass("email-toggled");
    });
    $(".email-toggle-btn-mobile").on("click", function () {
        $(".email-wrapper").removeClass("email-toggled");
    });

    // Mail Yazma Pop-up
    $(".compose-mail-btn").on("click", function () {
        $(".compose-mail-popup").show();
    });
    $(".compose-mail-close").on("click", function () {
        $(".compose-mail-popup").hide();
    });

    // Renk/Tema Deðiþtirici (Switcher)
    $(".switcher-btn").on("click", function () {
        $(".switcher-wrapper").toggleClass("switcher-toggled");
    });
    $(".close-switcher").on("click", function () {
        $(".switcher-wrapper").removeClass("switcher-toggled");
    });

});