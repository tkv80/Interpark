using System;
using System.Threading;
using System.Windows.Forms;
using Interpark.Manager;

namespace Interpark
{
    public partial class Main : Form
    {
        private string cookie = "";

        public Main()
        {
            InitializeComponent();
            /*
            var f1 = "Tiki=N&TikiGrade=&TikiDiscountCnt=0&BizCode=WEBBR&GoodsBizCode=07922&GoodsBizName=%B7%D1%B8%B5%C8%A6%BF%A3%C5%CD%C5%D7%C0%CE%B8%D5%C6%AE&BizMemberCode=051050794&SessionId=F9E86CAC3BBE417BAFB40BF8BE9B6597&BadUserOrNot=N&TMemHash=42f1ea08ea0e7d6d870389254a8e61db&SIDBizCode=WEBBR&TicketAmt=198000&TicketCnt=2&TotalAmt=202500&TotalUseAmt=2000&DelyAmt=2500&VoucherAmt=0&GroupCode=14007567&GoodsCode=14007567&PlaceCode=14000794&OnlyDeliver=68001&DBDay=12&ExpressDelyDay=0&DelyFee=2500&ExpressDelyFee=0&ReservedOrNot=N&Always=N&GroupName=2014+%B7%BF%C3%F7%B6%F4+%C6%E4%BD%BA%C6%BC%B9%FA+Vol.8+-+%B0%F8%BD%C4+%C6%BC%C4%CF&PlaceName=%B3%AD%C1%F6%C7%D1%B0%AD%B0%F8%BF%F8+%B3%BB+%C1%DF%BE%D3%C0%DC%B5%F0%B1%A4%C0%E5+%2F+%C0%DC%B5%F0%B8%B6%B4%E7&LocOfImage=&TmgsOrNot=D2001&KindOfGoods=01003&SubCategory=03012&GoodsOption=67001&UseAmt=1000&TypeOfUse=P&PenaltyScript=%C6%BC%C4%CF%B1%DD%BE%D7%C0%C7+0%7E30%25&SupplyType=71001&RcptTaxYN=Y&HotSaleOrNot=N&AgreeOrNot=N&UserInfoOrNot=N&SeasonOrNot=N&StartDate=20140920&EndDate=20140921&PackageOrNot=N&MobileOrNot=N&HomeOrNot=N&SecretGoodsYN=N&LuckyBagGoodsYN=N&PlayDate=20140920&PlaySeq=001&PlayTime=12%BD%C3+30%BA%D0+&NoOfTime=1&OnlineDate=20140916&CancelableDate=201409191700&BookingEndDate=201409191700&LmtKindOfSettle=&LmtCardIssueCode=&LmtSalesYN=&KindOfDiscount=&DiscountName=&DiscountAmt=198000&DiscountSeq=&TikiKindOfDiscount=&OtherKindOfDiscount=&CardKindOfDiscount=&CardKind=&CardDiscountKind=&CardIssueCode=&CardDiscountNo=&CardDelivery=&Point=&PointSeq=&PointDiscountAmt=&CouponCode=&CouponName=&Coupon_DiscountValue=0&Coupon_TypeOfPay=&Coupon_ChargeInfo=&Coupon_DblDiscountOrNot=&Coupon_DiscountAmt=0&DeliveryOrNot=Y&DeliveryGift=&DeliveryGiftAmt=0&Delivery=24001&RName=%B1%E8%C5%C2%B1%C7&RPhoneNo=HXmh7wyPTPMVAHTH6ga0vw%3D%3D&RHpNo=LobpS%2FzjibGXLJuU6JW4ig%3D%3D&RZipcode=150-716&RAddr=%BC%AD%BF%EF+%BF%B5%B5%EE%C6%F7%B1%B8+%BF%A9%C0%C7%B5%B5%B5%BF+%B4%EB%BF%EC%C1%F5%B1%C7%BA%F4%B5%F9&RSubAddr=13%C3%FE+iMBC+%BD%C3%BD%BA%C5%DB+%B0%B3%B9%DF%C6%C0+%B1%E8%C5%C2%B1%C7&MemberName=%B1%E8%C5%C2%B1%C7&SSN=Gs0vY8kozP1o7xDBI9zoSw%3D%3D&PhoneNo=HXmh7wyPTPMVAHTH6ga0vw%3D%3D&HpNo=LobpS%2FzjibGXLJuU6JW4ig%3D%3D&Email=hqI6kQqdExxuskk6q23jrUQVSdTQ1RpbWcMs7Hq9hPQ%3D&SmsOrNot=Y&Zipcode=150-716&Addr=%BC%AD%BF%EF+%BF%B5%B5%EE%C6%F7%B1%B8+%BF%A9%C0%C7%B5%B5%B5%BF+%B4%EB%BF%EC%C1%F5%B1%C7%BA%F4%B5%F9&SubAddr=13%C3%FE+iMBC+%BD%C3%BD%BA%C5%DB+%B0%B3%B9%DF%C6%C0+%B1%E8%C5%C2%B1%C7&DeliveryEnc=Y&MCash_TelKind=&MCash_No=&MCash_SSN=&MCash_Smsval=&MCash_Phoneid=&MCash_Traid=&MCash_UseAmt=0&MCash_TotalAmt=GCnHhUHDaG1ech4k%2F69pZg%3D%3D&KindOfSettle=&CartID=        &CartIDSeq=     &DoubleChecked=&PaymentDouble=&TotGiftSttledAmt=&ISPDiscountOrNot=&DiscountOrNot=&TicketCntTikiDiscount=&FirstKindOfPayment=22004&SecondKindOfPayment=&FirstSettleAmt=202500&SecondSettleAmt=0&BankCode=38052&KindOfCard=&DiscountCard=&CardNo=&ValidInfo=&CardSSN=&CardPWD=&HalbuMonth=&LGPoint=&WooriPoint=&CitiPoint=&KBPoint=&MPoint=&SSPoint=&KEBPoint=&HashPrice=a496dfecd7457429e2a184e35ee4ef40&CultrueUser=&HappyUser=&FirstSubKindOfPayment=&PaymentEnc=Y&PayDiscountCode=&Encrypted=&RN=&IspCardCode=&CardCode=&CardQuota=&BearsNumber=&GSPwd=&GSCardNo=&GSType=&GSPointUseYN=&GSPointSaveYN=&useGSPoint=0&GSPointCheckYN=&OtherSiteGSCardNo=&OtherSiteGSYN=Y&OtherSiteGSPwd=&expoRecommander=&expoRecOrg=&expoName=&expoImg=&FCSeasonNo=&PGType=PG002&SeatGrade=1&PriceGrade=01&SalesPrice=99000&DblDiscountOrNot=N&GroupId=00000&SeatGradeName=2%C0%CF%B1%C7&PriceGradeName=%B0%F8%BD%C4%C6%BC%C4%CF&BlockNo=+&Floor=+&RowNo=+&SeatNo=+&VoucherCode=+&VoucherSalesPrice=0&DiscountCode=00000&SeatInfo=+&PGType=PG002&SeatGrade=1&PriceGrade=01&SalesPrice=99000&DblDiscountOrNot=N&GroupId=00000&SeatGradeName=2%C0%CF%B1%C7&PriceGradeName=%B0%F8%BD%C4%C6%BC%C4%CF&BlockNo=+&Floor=+&RowNo=+&SeatNo=+&VoucherCode=+&VoucherSalesPrice=0&DiscountCode=00000&SeatInfo=+";
            var f2 = "Tiki=N&TikiGrade=&TikiDiscountCnt=0&BizCode=WEBBR&GoodsBizCode=07922&GoodsBizName=%b7%d1%b8%b5%c8%a6%bf%a3%c5%cd%c5%d7%c0%ce%b8%d5%c6%ae&BizMemberCode=091020484&SessionId=C80AE93BB0A84EAEAC2804EA5DCC0707&BadUserOrNot=N&TMemHash=980fe4a4ab73ccd62c64753c8040d47e&SIDBizCode=WEBBR&TicketAmt=198000&TicketCnt=2&TotalAmt=202500&TotalUseAmt=2000&DelyAmt=2500&VoucherAmt=0&GroupCode=14007567&GoodsCode=14007567&PlaceCode=14000794&OnlyDeliver=68001&DBDay=12&ExpressDelyDay=0&DelyFee=2500&ExpressDelyFee=0&ReservedOrNot=N&Always=N&GroupName=2014+%b7%bf%c3%f7%b6%f4+%c6%e4%bd%ba%c6%bc%b9%fa+Vol.8+-+%b0%f8%bd%c4+%c6%bc%c4%cf&PlaceName=%b3%ad%c1%f6%c7%d1%b0%ad%b0%f8%bf%f8+%b3%bb+%c1%df%be%d3%c0%dc%b5%f0%b1%a4%c0%e5+%2f+%c0%dc%b5%f0%b8%b6%b4%e7&LocOfImage=&TmgsOrNot=D2001&KindOfGoods=01003&SubCategory=03012&GoodsOption=67001&UseAmt=1000&TypeOfUse=P&PenaltyScript=%c6%bc%c4%cf%b1%dd%be%d7%c0%c7+0%7e30%25&SupplyType=71001&RcptTaxYN=Y&HotSaleOrNot=N&AgreeOrNot=N&UserInfoOrNot=N&SeasonOrNot=N&StartDate=20140920&EndDate=20140921&PackageOrNot=N&MobileOrNot=N&HomeOrNot=N&SecretGoodsYN=N&LuckyBagGoodsYN=N&PlayDate=20140920&PlaySeq=001&PlayTime=12%bd%c3+30%ba%d0&NoOfTime=1&OnlineDate=20140916&CancelableDate=201409191700&BookingEndDate=201409191700&LmtKindOfSettle=&LmtCardIssueCode=&LmtSalesYN=N&KindOfDiscount=&DiscountName=&DiscountAmt=198000&DiscountSeq=&TikiKindOfDiscount=&OtherKindOfDiscount=&CardKindOfDiscount=&CardKind=&CardDiscountKind=&CardIssueCode=&CardDiscountNo=&CardDelivery=&Point=&PointSeq=&PointDiscountAmt=&CouponCode=&CouponName=&Coupon_DiscountValue=0&Coupon_TypeOfPay=&Coupon_ChargeInfo=&Coupon_DblDiscountOrNot=&Coupon_DiscountAmt=0&DeliveryOrNot=Y&DeliveryGift=&DeliveryGiftAmt=0&Delivery=24001&RName=%b3%b2%bc%d2%bf%b5&RPhoneNo=QVGJbNjkvg2%2bsUcOPfQ%2buw%3d%3d&RHpNo=%2f%2bDrsrn8F%2biyS2wvnGUciA%3d%3d&RZipcode=152-851&RAddr=%bc%ad%bf%ef+%b1%b8%b7%ce%b1%b8+%b1%b8%b7%ce2%b5%bf&RSubAddr=399&MemberName=%b3%b2%bc%d2%bf%b5&SSN=axQ%2b4NGYl5oGaelA%2bJMXrg%3d%3d&PhoneNo=QVGJbNjkvg2%2bsUcOPfQ%2buw%3d%3d&HpNo=%2f%2bDrsrn8F%2biyS2wvnGUciA%3d%3d&Email=h2Xq%2bAd59CV%2fyV%2fIIOOHED8cZnVRhktSvoLsuv3wK70%3d&SmsOrNot=Y&Zipcode=152-851&Addr=%bc%ad%bf%ef+%b1%b8%b7%ce%b1%b8+%b1%b8%b7%ce2%b5%bf&SubAddr=399&DeliveryEnc=Y&MCash_TelKind=&MCash_No=&MCash_SSN=&MCash_Smsval=&MCash_Phoneid=&MCash_Traid=&MCash_UseAmt=0&MCash_TotalAmt=ZUFKwG1e9t6%2fWYxgHvY%2bFw%3d%3d&KindOfSettle=&CartID=20140807&CartIDSeq=47391&DoubleChecked=&PaymentDouble=&TotGiftSttledAmt=&ISPDiscountOrNot=N&DiscountOrNot=N&TicketCntTikiDiscount=0&FirstKindOfPayment=22004&SecondKindOfPayment=&FirstSettleAmt=202500&SecondSettleAmt=0&BankCode=38052&KindOfCard=&DiscountCard=&CardNo=&ValidInfo=&CardSSN=&CardPWD=&HalbuMonth=&LGPoint=&WooriPoint=&CitiPoint=&KBPoint=&MPoint=&SSPoint=&KEBPoint=&HashPrice=d7be8efb37e61793a0e1a789a411c64c&CultrueUser=&HappyUser=&FirstSubKindOfPayment=&PaymentEnc=Y&PayDiscountCode=&Encrypted=&RN=&IspCardCode=&CardCode=&CardQuota=&BearsNumber=&GSPwd=&GSCardNo=&GSType=&GSPointUseYN=&GSPointSaveYN=&useGSPoint=0&GSPointCheckYN=&OtherSiteGSCardNo=&OtherSiteGSYN=Y&OtherSiteGSPwd=&expoRecommander=&expoRecOrg=&expoName=&expoImg=&FCSeasonNo=&PGType=PG002&SeatGrade=1&PriceGrade=01&SalesPrice=99000&DblDiscountOrNot=N&GroupId=00000&SeatGradeName=2%c0%cf%b1%c7&PriceGradeName=%b0%f8%bd%c4%c6%bc%c4%cf&BlockNo=&Floor=&RowNo=&SeatNo=&VoucherCode=&VoucherSalesPrice=0&DiscountCode=00000&SeatInfo=&PGType=PG002&SeatGrade=1&PriceGrade=01&SalesPrice=99000&DblDiscountOrNot=N&GroupId=00000&SeatGradeName=2%c0%cf%b1%c7&PriceGradeName=%b0%f8%bd%c4%c6%bc%c4%cf&BlockNo=&Floor=&RowNo=&SeatNo=&VoucherCode=&VoucherSalesPrice=0&DiscountCode=00000&SeatInfo=";
            var dd =
                "Tiki=N&TikiGrade=&TikiDiscountCnt=0&BizCode=WEBBR&GoodsBizCode=07922&GoodsBizName=롤링홀엔터테인먼트&BizMemberCode=051050794&SessionId=1398136AB1C24841872E7969DFC37352&BadUserOrNot=N&TMemHash=42f1ea08ea0e7d6d870389254a8e61db&SIDBizCode=WEBBR&TicketAmt=198000&TicketCnt=2&TotalAmt=200000&TotalUseAmt=2000&DelyAmt=0&VoucherAmt=0&GroupCode=14007567&GoodsCode=14007567&PlaceCode=14000794&OnlyDeliver=68001&DBDay=12&ExpressDelyDay=0&DelyFee=2500&ExpressDelyFee=0&ReservedOrNot=N&Always=N&GroupName=2014 렛츠락 페스티벌 Vol.8 - 공식 티켓&PlaceName=난지한강공원 내 중앙잔디광장 / 잔디마당&LocOfImage=&TmgsOrNot=D2001&KindOfGoods=01003&SubCategory=03012&GoodsOption=67001&UseAmt=1000&TypeOfUse=P&PenaltyScript=티켓금액의 0~30%&SupplyType=71001&RcptTaxYN=Y&HotSaleOrNot=N&AgreeOrNot=N&UserInfoOrNot=N&SeasonOrNot=N&StartDate=20140920&EndDate=20140921&PackageOrNot=N&MobileOrNot=N&HomeOrNot=N&SecretGoodsYN=N&LuckyBagGoodsYN=N&PlayDate=20140920&PlaySeq=001&PlayTime=12시 30분 &NoOfTime=1&OnlineDate=20140916&CancelableDate=201409191700&BookingEndDate=201409191700&LmtKindOfSettle=&LmtCardIssueCode=&LmtSalesYN=&KindOfDiscount=&DiscountName=&DiscountAmt=198000&DiscountSeq=&TikiKindOfDiscount=&OtherKindOfDiscount=&CardKindOfDiscount=&CardKind=&CardDiscountKind=&CardIssueCode=&CardDiscountNo=&CardDelivery=&Point=&PointSeq=&PointDiscountAmt=&CouponCode=&CouponName=&Coupon_DiscountValue=0&Coupon_TypeOfPay=&Coupon_ChargeInfo=&Coupon_DblDiscountOrNot=&Coupon_DiscountAmt=0&DeliveryOrNot=&DeliveryGift=&DeliveryGiftAmt=0&Delivery=&RName=&RPhoneNo=&RHpNo=&RZipcode=&RAddr=&RSubAddr=&MemberName=&SSN=&PhoneNo=&HpNo=&Email=&SmsOrNot=&Zipcode=&Addr=&SubAddr=&DeliveryEnc=&MCash_TelKind=&MCash_No=&MCash_SSN=&MCash_Smsval=&MCash_Phoneid=&MCash_Traid=&MCash_UseAmt=0&MCash_TotalAmt=0&KindOfSettle=&CartID=&CartIDSeq=&DoubleChecked=&PaymentDouble=&TotGiftSttledAmt=&ISPDiscountOrNot=&DiscountOrNot=&TicketCntTikiDiscount=&FirstKindOfPayment=&SecondKindOfPayment=&FirstSettleAmt=&SecondSettleAmt=&BankCode=&KindOfCard=&DiscountCard=&CardNo=&ValidInfo=&CardSSN=&CardPWD=&HalbuMonth=&LGPoint=&WooriPoint=&CitiPoint=&KBPoint=&MPoint=&SSPoint=&KEBPoint=&HashPrice=&CultrueUser=&HappyUser=&FirstSubKindOfPayment=&PaymentEnc=&PayDiscountCode=&Encrypted=&RN=&IspCardCode=&CardCode=&CardQuota=&BearsNumber=&GSPwd=&GSCardNo=&GSType=&GSPointUseYN=&GSPointSaveYN=&useGSPoint=0&GSPointCheckYN=&OtherSiteGSCardNo=&OtherSiteGSYN=&OtherSiteGSPwd=&expoRecommander=&expoRecOrg=&expoName=&expoImg=&FCSeasonNo=&PGType=PG002&SeatGrade=1&PriceGrade=01&SalesPrice=99000&DblDiscountOrNot=N&GroupId=00000&SeatGradeName=2일권&PriceGradeName=공식티켓&BlockNo= &Floor= &RowNo= &SeatNo= &VoucherCode= &VoucherSalesPrice=0&DiscountCode=00000&SeatInfo= &PGType=PG002&SeatGrade=1&PriceGrade=01&SalesPrice=99000&DblDiscountOrNot=N&GroupId=00000&SeatGradeName=2일권&PriceGradeName=공식티켓&BlockNo= &Floor= &RowNo= &SeatNo= &VoucherCode= &VoucherSalesPrice=0&DiscountCode=00000&SeatInfo= ";

            dd =
                "Tiki=N&TikiGrade=&TikiDiscountCnt=0&BizCode=WEBBR&GoodsBizCode=07922&GoodsBizName=롤링홀엔터테인먼트&BizMemberCode=051050794&SessionId=C0F7E4CDC43F461F8FB43A7E7BABC0E9&BadUserOrNot=N&TMemHash=42f1ea08ea0e7d6d870389254a8e61db&SIDBizCode=WEBBR&TicketAmt=198000&TicketCnt=2&TotalAmt=202500&TotalUseAmt=2000&DelyAmt=2500&VoucherAmt=0&GroupCode=14007567&GoodsCode=14007567&PlaceCode=14000794&OnlyDeliver=68001&DBDay=12&ExpressDelyDay=0&DelyFee=2500&ExpressDelyFee=0&ReservedOrNot=N&Always=N&GroupName=2014 렛츠락 페스티벌 Vol.8 - 공식 티켓&PlaceName=난지한강공원 내 중앙잔디광장 / 잔디마당&LocOfImage=&TmgsOrNot=D2001&KindOfGoods=01003&SubCategory=03012&GoodsOption=67001&UseAmt=1000&TypeOfUse=P&PenaltyScript=티켓금액의 0~30%&SupplyType=71001&RcptTaxYN=Y&HotSaleOrNot=N&AgreeOrNot=N&UserInfoOrNot=N&SeasonOrNot=N&StartDate=20140920&EndDate=20140921&PackageOrNot=N&MobileOrNot=N&HomeOrNot=N&SecretGoodsYN=N&LuckyBagGoodsYN=N&PlayDate=20140920&PlaySeq=001&PlayTime=12시 30분 &NoOfTime=1&OnlineDate=20140916&CancelableDate=201409191700&BookingEndDate=201409191700&LmtKindOfSettle=&LmtCardIssueCode=&LmtSalesYN=&KindOfDiscount=&DiscountName=&DiscountAmt=198000&DiscountSeq=&TikiKindOfDiscount=&OtherKindOfDiscount=&CardKindOfDiscount=&CardKind=&CardDiscountKind=&CardIssueCode=&CardDiscountNo=&CardDelivery=&Point=&PointSeq=&PointDiscountAmt=&CouponCode=&CouponName=&Coupon_DiscountValue=0&Coupon_TypeOfPay=&Coupon_ChargeInfo=&Coupon_DblDiscountOrNot=&Coupon_DiscountAmt=0&DeliveryOrNot=Y&DeliveryGift=&DeliveryGiftAmt=0&Delivery=24001&RName=김태권&RPhoneNo=HXmh7wyPTPMVAHTH6ga0vw==&RHpNo=LobpS/zjibGXLJuU6JW4ig==&RZipcode=150-716&RAddr=서울 영등포구 여의도동 대우증권빌딩&RSubAddr=13층 iMBC 시스템 개발팀 김태권&MemberName=김태권&SSN=Gs0vY8kozP1o7xDBI9zoSw==&PhoneNo=HXmh7wyPTPMVAHTH6ga0vw==&HpNo=LobpS/zjibGXLJuU6JW4ig==&Email=hqI6kQqdExxuskk6q23jrUQVSdTQ1RpbWcMs7Hq9hPQ=&SmsOrNot=Y&Zipcode=150-716&Addr=서울 영등포구 여의도동 대우증권빌딩&SubAddr=13층 iMBC 시스템 개발팀 김태권&DeliveryEnc=Y&MCash_TelKind=&MCash_No=&MCash_SSN=&MCash_Smsval=&MCash_Phoneid=&MCash_Traid=&MCash_UseAmt=0&MCash_TotalAmt=0&KindOfSettle=&CartID=&CartIDSeq=&DoubleChecked=&PaymentDouble=&TotGiftSttledAmt=&ISPDiscountOrNot=&DiscountOrNot=&TicketCntTikiDiscount=&FirstKindOfPayment=&SecondKindOfPayment=&FirstSettleAmt=&SecondSettleAmt=&BankCode=&KindOfCard=&DiscountCard=&CardNo=&ValidInfo=&CardSSN=&CardPWD=&HalbuMonth=&LGPoint=&WooriPoint=&CitiPoint=&KBPoint=&MPoint=&SSPoint=&KEBPoint=&HashPrice=&CultrueUser=&HappyUser=&FirstSubKindOfPayment=&PaymentEnc=&PayDiscountCode=&Encrypted=&RN=&IspCardCode=&CardCode=&CardQuota=&BearsNumber=&GSPwd=&GSCardNo=&GSType=&GSPointUseYN=&GSPointSaveYN=&useGSPoint=0&GSPointCheckYN=&OtherSiteGSCardNo=&OtherSiteGSYN=Y&OtherSiteGSPwd=&expoRecommander=&expoRecOrg=&expoName=&expoImg=&FCSeasonNo=&PGType=PG002&SeatGrade=1&PriceGrade=01&SalesPrice=99000&DblDiscountOrNot=N&GroupId=00000&SeatGradeName=2일권&PriceGradeName=공식티켓&BlockNo= &Floor= &RowNo= &SeatNo= &VoucherCode= &VoucherSalesPrice=0&DiscountCode=00000&SeatInfo= &PGType=PG002&SeatGrade=1&PriceGrade=01&SalesPrice=99000&DblDiscountOrNot=N&GroupId=00000&SeatGradeName=2일권&PriceGradeName=공식티켓&BlockNo= &Floor= &RowNo= &SeatNo= &VoucherCode= &VoucherSalesPrice=0&DiscountCode=00000&SeatInfo=";
            dd =
                "Tiki=N&TikiGrade=&TikiDiscountCnt=0&BizCode=WEBBR&GoodsBizCode=07922&GoodsBizName=%B7%D1%B8%B5%C8%A6%BF%A3%C5%CD%C5%D7%C0%CE%B8%D5%C6%AE&BizMemberCode=051050794&SessionId=F9E86CAC3BBE417BAFB40BF8BE9B6597&BadUserOrNot=N&TMemHash=42f1ea08ea0e7d6d870389254a8e61db&SIDBizCode=WEBBR&TicketAmt=198000&TicketCnt=2&TotalAmt=202500&TotalUseAmt=2000&DelyAmt=2500&VoucherAmt=0&GroupCode=14007567&GoodsCode=14007567&PlaceCode=14000794&OnlyDeliver=68001&DBDay=12&ExpressDelyDay=0&DelyFee=2500&ExpressDelyFee=0&ReservedOrNot=N&Always=N&GroupName=2014+%B7%BF%C3%F7%B6%F4+%C6%E4%BD%BA%C6%BC%B9%FA+Vol.8+-+%B0%F8%BD%C4+%C6%BC%C4%CF&PlaceName=%B3%AD%C1%F6%C7%D1%B0%AD%B0%F8%BF%F8+%B3%BB+%C1%DF%BE%D3%C0%DC%B5%F0%B1%A4%C0%E5+%2F+%C0%DC%B5%F0%B8%B6%B4%E7&LocOfImage=&TmgsOrNot=D2001&KindOfGoods=01003&SubCategory=03012&GoodsOption=67001&UseAmt=1000&TypeOfUse=P&PenaltyScript=%C6%BC%C4%CF%B1%DD%BE%D7%C0%C7+0%7E30%25&SupplyType=71001&RcptTaxYN=Y&HotSaleOrNot=N&AgreeOrNot=N&UserInfoOrNot=N&SeasonOrNot=N&StartDate=20140920&EndDate=20140921&PackageOrNot=N&MobileOrNot=N&HomeOrNot=N&SecretGoodsYN=N&LuckyBagGoodsYN=N&PlayDate=20140920&PlaySeq=001&PlayTime=12%BD%C3+30%BA%D0+&NoOfTime=1&OnlineDate=20140916&CancelableDate=201409191700&BookingEndDate=201409191700&LmtKindOfSettle=&LmtCardIssueCode=&LmtSalesYN=&KindOfDiscount=&DiscountName=&DiscountAmt=198000&DiscountSeq=&TikiKindOfDiscount=&OtherKindOfDiscount=&CardKindOfDiscount=&CardKind=&CardDiscountKind=&CardIssueCode=&CardDiscountNo=&CardDelivery=&Point=&PointSeq=&PointDiscountAmt=&CouponCode=&CouponName=&Coupon_DiscountValue=0&Coupon_TypeOfPay=&Coupon_ChargeInfo=&Coupon_DblDiscountOrNot=&Coupon_DiscountAmt=0&DeliveryOrNot=Y&DeliveryGift=&DeliveryGiftAmt=0&Delivery=24001&RName=%B1%E8%C5%C2%B1%C7&RPhoneNo=HXmh7wyPTPMVAHTH6ga0vw%3D%3D&RHpNo=LobpS%2FzjibGXLJuU6JW4ig%3D%3D&RZipcode=150-716&RAddr=%BC%AD%BF%EF+%BF%B5%B5%EE%C6%F7%B1%B8+%BF%A9%C0%C7%B5%B5%B5%BF+%B4%EB%BF%EC%C1%F5%B1%C7%BA%F4%B5%F9&RSubAddr=13%C3%FE+iMBC+%BD%C3%BD%BA%C5%DB+%B0%B3%B9%DF%C6%C0+%B1%E8%C5%C2%B1%C7&MemberName=%B1%E8%C5%C2%B1%C7&SSN=Gs0vY8kozP1o7xDBI9zoSw%3D%3D&PhoneNo=HXmh7wyPTPMVAHTH6ga0vw%3D%3D&HpNo=LobpS%2FzjibGXLJuU6JW4ig%3D%3D&Email=hqI6kQqdExxuskk6q23jrUQVSdTQ1RpbWcMs7Hq9hPQ%3D&SmsOrNot=Y&Zipcode=150-716&Addr=%BC%AD%BF%EF+%BF%B5%B5%EE%C6%F7%B1%B8+%BF%A9%C0%C7%B5%B5%B5%BF+%B4%EB%BF%EC%C1%F5%B1%C7%BA%F4%B5%F9&SubAddr=13%C3%FE+iMBC+%BD%C3%BD%BA%C5%DB+%B0%B3%B9%DF%C6%C0+%B1%E8%C5%C2%B1%C7&DeliveryEnc=Y&MCash_TelKind=&MCash_No=&MCash_SSN=&MCash_Smsval=&MCash_Phoneid=&MCash_Traid=&MCash_UseAmt=0&MCash_TotalAmt=GCnHhUHDaG1ech4k%2F69pZg%3D%3D&KindOfSettle=&CartID=20140807&CartIDSeq=40367&DoubleChecked=&PaymentDouble=&TotGiftSttledAmt=&ISPDiscountOrNot=N&DiscountOrNot=N&TicketCntTikiDiscount=0&FirstKindOfPayment=22004&SecondKindOfPayment=&FirstSettleAmt=202500&SecondSettleAmt=0&BankCode=38052&KindOfCard=&DiscountCard=&CardNo=&ValidInfo=&CardSSN=&CardPWD=&HalbuMonth=&LGPoint=&WooriPoint=&CitiPoint=&KBPoint=&MPoint=&SSPoint=&KEBPoint=&HashPrice=a496dfecd7457429e2a184e35ee4ef40&CultrueUser=&HappyUser=&FirstSubKindOfPayment=&PaymentEnc=Y&PayDiscountCode=&Encrypted=&RN=&IspCardCode=&CardCode=&CardQuota=&BearsNumber=&GSPwd=&GSCardNo=&GSType=&GSPointUseYN=&GSPointSaveYN=&useGSPoint=0&GSPointCheckYN=&OtherSiteGSCardNo=&OtherSiteGSYN=Y&OtherSiteGSPwd=&expoRecommander=&expoRecOrg=&expoName=&expoImg=&FCSeasonNo=&PGType=PG002&SeatGrade=1&PriceGrade=01&SalesPrice=99000&DblDiscountOrNot=N&GroupId=00000&SeatGradeName=2%C0%CF%B1%C7&PriceGradeName=%B0%F8%BD%C4%C6%BC%C4%CF&BlockNo=+&Floor=+&RowNo=+&SeatNo=+&VoucherCode=+&VoucherSalesPrice=0&DiscountCode=00000&SeatInfo=+&PGType=PG002&SeatGrade=1&PriceGrade=01&SalesPrice=99000&DblDiscountOrNot=N&GroupId=00000&SeatGradeName=2%C0%CF%B1%C7&PriceGradeName=%B0%F8%BD%C4%C6%BC%C4%CF&BlockNo=+&Floor=+&RowNo=+&SeatNo=+&VoucherCode=+&VoucherSalesPrice=0&DiscountCode=00000&SeatInfo=+";
           IList<Tuple<string, string>> parameter =
                dd.Split('&').Select(d => d.Split('=')).Select(sp => new Tuple<string, string>(sp[0], sp[1])).ToList();
            var ss = parameter;

            var parameter1 = "";
            var parameter2 = "";
            var parameter3 = "";
            var i = 0;
            foreach (var s in ss)
            {
                //if (s.Item2.Trim() != "")
                {
                    //parameter1 += s.Item1 + "={" + i + "}&";
                    //parameter2 += s.Item1 + "=" + s.Item2 + "&";

                    
                    //else
                    {
                        parameter1 += s.Item1 + "={" + i + "}&";
                        parameter2 += "sessionInfo." + s.Item1 + ".UrlDecode(),";
                        i++;
                    }
                }
            }

            Console.WriteLine(parameter1);
            Console.WriteLine(parameter2);
            Console.WriteLine(parameter3);
            */
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var cookies = HttpManager.LogIn(txtId.Text, txtPassword.Text);

            if (cookies == null)
            {
                MessageBox.Show(@"유효한 회원 정보가 아닙니다."); Util.Logging(this.txtLog, "유효한 회원 정보가 아닙니다.");
                return;
            }
            
            foreach (var cookie1 in cookies)
            {
                var splitString = cookie1.Split(';');
                cookie += splitString[0] + ";";
            }

            Util.Logging(this.txtLog, "로그인 완료");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var goods = HttpManager.GetInfo(txtGoodsCode.Text);
            txtTitle.Text = goods.GoodsName;

            
        }

        private void btnStartReservation_Click(object sender, EventArgs e)
        {
            if (btnStartReservation.Text == "예약중지")
            {
                btnStartReservation.Text = "예약시작";
            }
            else
            {
                btnStartReservation.Text = "예약중지";
            }

            while (true)
            {
                if (btnStartReservation.Text == "예약시작")
                {
                    return;
                }
                if (HttpManager.GetGoodsOpen(txtGoodsCode.Text) || DateTime.Now > new DateTime(2014,8,12,14,0,0))
                {
                    // 1. 세션정보 조회
                    var sessionInfo = HttpManager.GetSessinInfo(txtGoodsCode.Text, ref cookie);

                    // 2. 상품 정보 조회
                    var result = HttpManager.BookMain(ref sessionInfo, ref cookie);
                    if (result)
                    {
                        // 3. 상품 정보 조회
                        HttpManager.GetBaseData(ref sessionInfo, ref cookie);

                        //Thread.Sleep(200);

                        // 3-1. 상품 정보 조회
                        HttpManager.GetBaseXmlData(ref sessionInfo, ref cookie);
                        //Thread.Sleep(200);

                        // 3-2. 상품 정보 조회
                        HttpManager.GetBaseXmlData2(ref sessionInfo, ref cookie);
                        //Thread.Sleep(200);

                        // 4. 티켓정보
                        HttpManager.GetTicketInfo(ref sessionInfo, ref cookie);
                        //Thread.Sleep(200);

                        // 5. 패널티 스크립트
                        HttpManager.GetPanaltyScript(ref sessionInfo);
                        //Thread.Sleep(200);

                        // 6. 유저정보 조회
                        result = HttpManager.GetUserInfo(ref sessionInfo, ref cookie);

                        if (result)
                        {
                            // 7. 유저 데이터 암호화
                            HttpManager.GetEncryptData(ref sessionInfo, ref cookie);

                            //8. 결제 정보 조회
                            HttpManager.GetPayment(ref sessionInfo, ref cookie);

                            //9. Mcache 암호화
                            HttpManager.GetMCacheEncryptData(ref sessionInfo, ref cookie);

                            //9. 확인
                            HttpManager.Confirm(ref sessionInfo, ref cookie);

                            //10. 완료
                            result = HttpManager.End(ref sessionInfo, ref cookie);

                            if (result)
                            {
                                MessageBox.Show("예약완료");
                                Util.Logging(this.txtLog, txtTitle.Text + " " + "예약성공");
                                new GcmManager().SendNotification("인터파크 예약완료", "성공");
                                btnStartReservation.Text = "예약시작";
                                break;
                            }
                        }
                    }
                }
                Util.Logging(this.txtLog, "오픈전");
                Thread.Sleep(500);
            }
        }
    }
}
