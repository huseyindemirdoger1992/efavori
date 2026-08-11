using Microsoft.AspNetCore.Mvc;

namespace web.Areas.Public.Controllers
{
    // ═══════════════════════════════════════════════════════════════════════
    //  FixedPages — Sabit / kurumsal içerik sayfaları (efavori.com)
    //  ─────────────────────────────────────────────────────────────────────
    //  FooterArea.razor'daki statik linkler tek noktadan bu controller üzerinden
    //  karşılanır. Yalnızca kullanıcıya göre değişmeyen; DB listesi, oturum veya
    //  form-POST gerektirmeyen bilgilendirme / pazarlama / yasal sayfalar burada.
    //
    //  Her aksiyonun üstündeki yorum, footer'daki bağlantı metnini gösterir.
    // ═══════════════════════════════════════════════════════════════════════
    [Area("Public")]
    [Route("{culture}/Public/[controller]/[action]")]
    public class FixedPages : Controller
    {

        // ── Kurumsal ────────────────────────────────────────────────────────

        // Hakkımızda
        public IActionResult About()
        {
            return View();
        }

        // Kariyer
        public IActionResult Careers()
        {
            return View();
        }

        // İletişim
        public IActionResult Contact()
        {
            return View();
        }

        // Basın Odası
        public IActionResult Press()
        {
            return View();
        }

        // Sürdürülebilirlik
        public IActionResult Sustainability()
        {
            return View();
        }

        // Yatırımcı İlişkileri
        public IActionResult InvestorRelations()
        {
            return View();
        }

        // ── Yardım ve Destek ────────────────────────────────────────────────

        // Yardım Merkezi
        public IActionResult HelpCenter()
        {
            return View();
        }

        // İade ve Değişim
        public IActionResult Returns()
        {
            return View();
        }

        // Kargo ve Teslimat
        public IActionResult Shipping()
        {
            return View();
        }

        // Sıkça Sorulan Sorular
        public IActionResult Faq()
        {
            return View();
        }

        // Talep ve Şikâyetler
        public IActionResult Complaints()
        {
            return View();
        }

        // ── efavori'de Satış Yap ────────────────────────────────────────────

        // Mağaza Aç
        public IActionResult Sell()
        {
            return View();
        }

        // Satıcı Akademisi
        public IActionResult SellerAcademy()
        {
            return View();
        }

        // Komisyon Oranları
        public IActionResult SellerCommissions()
        {
            return View();
        }

        // Kurumsal Satış
        public IActionResult Business()
        {
            return View();
        }

        // İş Ortaklığı Programı
        public IActionResult Affiliate()
        {
            return View();
        }

        // Reklam Ver
        public IActionResult Advertising()
        {
            return View();
        }

        // ── Ödeme ve Avantajlar ─────────────────────────────────────────────

        // Ödeme Seçenekleri
        public IActionResult PaymentMethods()
        {
            return View();
        }

        // Taksit Seçenekleri
        public IActionResult PaymentInstallments()
        {
            return View();
        }

        // Hediye Kartı
        public IActionResult GiftCard()
        {
            return View();
        }

        // Alışveriş Kredisi
        public IActionResult PaymentCredit()
        {
            return View();
        }

        // ── Kargo ve Lojistik ───────────────────────────────────────────────

        // Teslimat Süreleri
        public IActionResult DeliveryTimes()
        {
            return View();
        }

        // Kargo Ücretleri
        public IActionResult ShippingFees()
        {
            return View();
        }

        // Aynı Gün Teslimat
        public IActionResult SameDayDelivery()
        {
            return View();
        }

        // Teslimat Noktaları
        public IActionResult PickupPoints()
        {
            return View();
        }

        // Yurt Dışı Gönderim
        public IActionResult InternationalShipping()
        {
            return View();
        }

        // Anlaşmalı Kargo Firmaları
        public IActionResult Carriers()
        {
            return View();
        }

        // ── Güvenlik ve Yasal (Footer sütunu) ───────────────────────────────

        // Güvenli Alışveriş Rehberi
        public IActionResult SafeShopping()
        {
            return View();
        }

        // Alıcı Koruma Programı
        public IActionResult BuyerProtection()
        {
            return View();
        }

        // Gizlilik Merkezi
        public IActionResult PrivacyCenter()
        {
            return View();
        }

        // Fikri Mülkiyet Hakları
        public IActionResult IpProtection()
        {
            return View();
        }

        // Yasaklı ve Kısıtlı Ürünler
        public IActionResult ProhibitedItems()
        {
            return View();
        }

        // Şeffaflık Raporu
        public IActionResult Transparency()
        {
            return View();
        }

        // ETBİS Kaydı
        public IActionResult Etbis()
        {
            return View();
        }

        // ── Yasal (Alt bar) ─────────────────────────────────────────────────

        // Kullanım Koşulları
        public IActionResult Terms()
        {
            return View();
        }

        // Gizlilik Politikası
        public IActionResult Privacy()
        {
            return View();
        }

        // Çerez Politikası
        public IActionResult Cookies()
        {
            return View();
        }

        // KVKK Aydınlatma Metni
        public IActionResult Kvkk()
        {
            return View();
        }

        // Mesafeli Satış Sözleşmesi
        public IActionResult DistanceSales()
        {
            return View();
        }

        // Üyelik Sözleşmesi
        public IActionResult Membership()
        {
            return View();
        }

        // Çerez Tercihleri
        public IActionResult CookieSettings()
        {
            return View();
        }

        // Erişilebilirlik
        public IActionResult Accessibility()
        {
            return View();
        }
    }
}