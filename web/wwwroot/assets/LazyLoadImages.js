/* ==========================================================
   EFAVORI – GLOBAL IMAGE LAZY LOAD ENGINE
   Uyumlu: Blazor Server / Razor / MVC / HTML
   Performans: Lighthouse uyumlu
   CLS: Önlenmiş
   ========================================================== */

(function () {

    /**
     * Lazy uygulanacak img'leri bul ve işle
     */
    function applyLazyImages(context = document) {

        if (!context || !context.querySelectorAll) return;

        const images = context.querySelectorAll(
            "img:not([loading]):not(.no-lazy)"
        );

        images.forEach(img => {

            // SVG genelde küçük – hariç tut
            if (img.src && img.src.endsWith(".svg")) return;

            // Lazy ata
            img.setAttribute("loading", "lazy");

            // CLS önleme (layout shift)
            if (!img.hasAttribute("width") && img.naturalWidth > 0) {
                img.setAttribute("width", img.naturalWidth);
            }
            if (!img.hasAttribute("height") && img.naturalHeight > 0) {
                img.setAttribute("height", img.naturalHeight);
            }

        });
    }

    /* ------------------------------------------------------
       GLOBAL AÇ (HTML / Blazor / JS Interop için)
    ------------------------------------------------------ */
    window.applyLazyImages = applyLazyImages;

    /* ------------------------------------------------------
       İlk sayfa yükü
    ------------------------------------------------------ */
    window.addEventListener("load", function () {
        applyLazyImages();
    });

    /* ------------------------------------------------------
       Blazor / SPA / AJAX DOM değişikliklerini izle
    ------------------------------------------------------ */
    const observer = new MutationObserver(mutations => {
        mutations.forEach(mutation => {
            mutation.addedNodes.forEach(node => {
                if (node.nodeType !== 1) return;

                // Tek img geldiyse
                if (node.tagName === "IMG") {
                    applyLazyImages(node.parentNode);
                }

                // İçinde img varsa
                if (node.querySelectorAll) {
                    applyLazyImages(node);
                }
            });
        });
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true
    });

})();
