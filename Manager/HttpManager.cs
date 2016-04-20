using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Interpark.Entity;

namespace Interpark.Manager
{
    internal static class HttpManager
    {

        /// <summary>
        /// 상품 오픈 여부 조회
        /// </summary>
        /// <param name="goodsCode"></param>
        /// <returns></returns>
        public static bool GetGoodsOpen(string goodsCode)
        {
            try
            {
                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create(string.Format(
                                "http://ticket.interpark.com/Ticket/Goods/GoodsInfo.asp?GoodsCode={0}", goodsCode));
                httpWRequest.Method = "get";

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                string resultHtml = sr.ReadToEnd();
                resultHtml = resultHtml.Substring("<div class=\"infoCell\">", "</div>");
                if (resultHtml.Contains("판매 예정인 상품입니다."))
                {
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                return true;
            }
        }

        /// <summary>
        /// 쿠키 조회
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> LogIn(string id, string password)
        {
            try
            {
                string parameter =
                    string.Format(
                        "autologin=&saveMemId=&MemBizCD=WEBBR&OtherMem=&UID={0}&PWD={1}",
                        id, password);

                var httpWRequest =
                    (HttpWebRequest)WebRequest.Create("https://ticket.interpark.com/Gate/TPLoginConfirm.asp");
                httpWRequest.Accept = "text/html, application/xhtml+xml, */*";
                httpWRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                httpWRequest.Headers.Add("Accept-Language", "ko-KR");
                httpWRequest.Referer = "https://ticket.interpark.com/Gate/TPLogOut.asp?From=T&tid1=main_gnb&tid2=right_top&tid3=logout&tid4=logout";
                httpWRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
                httpWRequest.Headers.Add("DNT", "1");
                httpWRequest.ContentType = "application/x-www-form-urlencoded;";
                httpWRequest.KeepAlive = true;
                httpWRequest.Method = "Post";
                httpWRequest.ContentLength = parameter.Length;
                httpWRequest.CookieContainer = new CookieContainer();

                var sw = new StreamWriter(httpWRequest.GetRequestStream());
                sw.Write(parameter);
                sw.Close();

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream());

                sr.ReadToEnd();
                return theResponse.Headers.GetValues("Set-Cookie");
            }
            catch (Exception ex)
            {
                Util.ErrorLog(MethodBase.GetCurrentMethod(), ex, "에러");
                return null;
            }
        }

        /// <summary>
        /// 세션정보
        /// </summary>
        /// <returns></returns>
        public static SessionInfo GetSessinInfo(string goodsCode, ref string cookie)
        {
            try
            {
                string parameter =
                   string.Format(
                       "GroupCode={0}&Tiki=N&Point=N&PlayDate=&PlaySeq=&BizCode=&BizMemberCode=",
                       goodsCode);

                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create("http://ticket.interpark.com/Book/BookSession.asp");
                httpWRequest.Accept = "text/html, application/xhtml+xml, */*";
                httpWRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                httpWRequest.Headers.Add("Accept-Language", "ko-KR");
                httpWRequest.Referer = "http://ticket.interpark.com/Ticket/Goods/GoodsInfo.asp?GoodsCode=" + goodsCode;
                httpWRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
                httpWRequest.Headers.Add("DNT", "1");
                httpWRequest.ContentType = "application/x-www-form-urlencoded;";
                httpWRequest.KeepAlive = true;
                httpWRequest.Method = "Post";
                httpWRequest.ContentLength = parameter.Length;
                httpWRequest.CookieContainer = new CookieContainer();

                Util.SetCookie(cookie, httpWRequest);

                var sw = new StreamWriter(httpWRequest.GetRequestStream());
                sw.Write(parameter);
                sw.Close();

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                string resultHtml = sr.ReadToEnd();

                var reservation =new SessionInfo();
                reservation.GroupCode = resultHtml.Substring("name=\"GroupCode\" value=\"", "\">");
                reservation.Tiki = resultHtml.Substring("name=\"Tiki\" value=\"", "\">");
                reservation.BizCode = resultHtml.Substring("name=\"BizCode\" value=\"", "\">");
                reservation.BizMemberCode = resultHtml.Substring("name=\"BizMemberCode\" value=\"", "\">");
                reservation.PlayDate = resultHtml.Substring("name=\"PlayDate\" value=\"", "\">");
                reservation.PlaySeq = resultHtml.Substring("name=\"PlaySeq\" value=\"", "\">");
                reservation.SessionId = resultHtml.Substring("name=\"SessionId\" value=\"", "\">");
                reservation.SIDBizCode = resultHtml.Substring("name=\"SIDBizCode\" value=\"", "\">");
                reservation.FCSeasonNo = resultHtml.Substring("name=\"FCSNo\" value=\"", "\">");

                
                var interparkstamp= theResponse.Headers.GetValues("Set-Cookie").FirstOrDefault(x => x.Contains("interparkstamp"));
                interparkstamp = interparkstamp.Substring("interparkstamp=", ";");

                var searchText = cookie.Substring("interparkstamp=", ";");
                cookie.Replace(searchText, interparkstamp);
                return reservation;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 상품 이름 조회
        /// </summary>
        /// <param name="goodsCode"></param>
        /// <returns></returns>
        public static Goods GetInfo(string goodsCode)
        {
            try
            {
                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create(string.Format(
                                "http://ticket.interpark.com/Ticket/Goods/GoodsInfo.asp?GoodsCode={0}", goodsCode));
                httpWRequest.Method = "get";
                httpWRequest.Referer = "http://ticket.interpark.com/Ticket/Goods/GoodsInfo.asp?GoodsCode=" + goodsCode;
                httpWRequest.Referer = "https://ticket.interpark.com/Gate/TPLogOut.asp?From=T&tid1=main_gnb&tid2=right_top&tid3=logout&tid4=logout";
                
                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                string resultHtml = sr.ReadToEnd();
                var goods = new Goods();

                goods.GoodsName = resultHtml.Substring("var vGN = \"", "\";");
                goods.PlaceCode= resultHtml.Substring("var vPC = \"", "\";");
                return goods;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 1. 상품 기본정보
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool BookMain(ref SessionInfo sessionInfo, ref string cookie)
        {
            try
            {
                string parameter =
                    string.Format(
                        "GroupCode={0}&Tiki={1}&BizCode={2}&BizMemberCode={3}&PlayDate={4}&PlaySeq={5}&SessionId={6}&SIDBizCode={7}&FCSNo={8}",
                        sessionInfo.GroupCode,
                        sessionInfo.Tiki,
                        sessionInfo.BizCode,
                        sessionInfo.BizMemberCode,
                        sessionInfo.PlayDate,
                        sessionInfo.PlaySeq,
                        sessionInfo.SessionId,
                        sessionInfo.SIDBizCode,
                        sessionInfo.FCSeasonNo);

                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create("http://ticket.interpark.com/Book/BookMain.asp");
                httpWRequest.Accept = "text/html, application/xhtml+xml, */*";
                httpWRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                httpWRequest.Headers.Add("Accept-Language", "ko-KR");
                httpWRequest.Referer = "http://ticket.interpark.com/Book/BookSession.asp";
                httpWRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
                httpWRequest.Headers.Add("DNT", "1");
                httpWRequest.ContentType = "application/x-www-form-urlencoded;";
                httpWRequest.KeepAlive = true;
                httpWRequest.Method = "Post";
                httpWRequest.ContentLength = parameter.Length;
                httpWRequest.CookieContainer = new CookieContainer();
                httpWRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Util.SetCookie(cookie, httpWRequest);

                var sw = new StreamWriter(httpWRequest.GetRequestStream(), Encoding.GetEncoding("euc-kr"));
                sw.Write(parameter);
                sw.Close();

                var theResponse = (HttpWebResponse) httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                string resultHtml = sr.ReadToEnd();

                if (resultHtml.Contains("상품정보가 올바르지 않습니다"))
                {
                    return false;
                }

                var valueSearch =
                    resultHtml.Substring(
                        "<form name=\"formBook\" id=\"formBook\" method=\"post\" onsubmit=\"\" action=\"\" target=\"\">",
                        "</form>");

                foreach (var propertyInfo in typeof (SessionInfo).GetProperties())
                {
                    if (propertyInfo.Name == "DBDay")
                    {
                    }
                    if (valueSearch.IndexOf("name=\"" + propertyInfo.Name + "\" value=\"") > -1)
                    {
                        var value = valueSearch.Substring("name=\"" + propertyInfo.Name + "\" value=\"", "\">");

                        var property = sessionInfo.GetType().GetProperty(propertyInfo.Name);
                        if (property.GetValue(sessionInfo, null) == null)
                        {
                            property.SetValue(sessionInfo, value);
                        }
                    }
                }

                var playDate = resultHtml.Substring(
                    "<select style=\"width:150px;\" id=\"SelPlayDate\" name=\"SelPlayDate\" onchange=\"fnOtherPlaySeqSelect()\">",
                    "</select>");
                sessionInfo.PlayDate = playDate.Substring("value=\"", "\" ");
                sessionInfo.PenaltyScript = "티켓금액의 0~30%";


                var interparkstamp = theResponse.Headers.GetValues("Set-Cookie").FirstOrDefault(x => x.Contains("interparkstamp"));
                interparkstamp = interparkstamp.Substring("interparkstamp=", ";");

                var searchText = cookie.Substring("interparkstamp=", ";");
                cookie.Replace(searchText, interparkstamp);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 2. 기초 정보 조회
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static void GetBaseData(ref SessionInfo sessionInfo, ref string cookie)
        {
            try
            {
                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create(string.Format(
                                "http://ticket.interpark.com/Book/BookDatetime.asp?GoodsCode={0}&PlaceCode={1}&OnlyDeliver={2}&DBDay={3}&ExpressDelyDay={4}&Tiki={5}&BizCode={6}&BizMemberCode={7}&KindOfGoods={8}&Always={9}&HotSaleOrNot={10}&PlayDate=&PlaySeq=&SessionId={11}",
                                sessionInfo.GoodsCode, sessionInfo.PlaceCode, sessionInfo.OnlyDeliver, sessionInfo.DBDay, sessionInfo.ExpressDelyDay, sessionInfo.Tiki, sessionInfo.BizCode, sessionInfo.BizMemberCode, sessionInfo.KindOfGoods, sessionInfo.Always, sessionInfo.HotSaleOrNot, sessionInfo.SessionId));
                httpWRequest.Method = "get";
                httpWRequest.Referer = "https://ticket.interpark.com/Gate/TPLogOut.asp?From=T&tid1=main_gnb&tid2=right_top&tid3=logout&tid4=logout";
                httpWRequest.CookieContainer = new CookieContainer();

                Util.SetCookie(cookie, httpWRequest);

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                string resultHtml = sr.ReadToEnd();
                var valueSearch =
                   resultHtml.Substring(
                       "<form id=\"formCalendar\" name=\"formCalendar\" method=\"get\" action=\"BookDateTime.asp\">",
                       "</form>");
                foreach (var propertyInfo in typeof(SessionInfo).GetProperties())
                {

                    if (valueSearch.IndexOf("name=\"" + propertyInfo.Name + "\" value=\"") > -1)
                    {
                        var value = valueSearch.Substring("name=\"" + propertyInfo.Name + "\" value=\"", "\">");

                        var property = sessionInfo.GetType().GetProperty(propertyInfo.Name);
                        if (property.GetValue(sessionInfo, null) == null)
                        {
                            property.SetValue(sessionInfo, value);
                        }
                    }
                }

                var interparkstamp = theResponse.Headers.GetValues("Set-Cookie").FirstOrDefault(x => x.Contains("interparkstamp"));
                interparkstamp = interparkstamp.Substring("interparkstamp=", ";");

                var searchText = cookie.Substring("interparkstamp=", ";");
                cookie.Replace(searchText, interparkstamp);

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 2-1. 기초 정보 조회
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static void GetBaseXmlData(ref SessionInfo sessionInfo, ref string cookie)
        {
            try
            {
                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create(string.Format(
                                "http://ticket.interpark.com/Book/Lib/BookInfoXml.asp?Flag=PlaySeq&GoodsCode={0}&PlaceCode={1}&OnlyDeliver={2}&DBDay={3}&ExpressDelyDay={4}&BizCode={5}&BizMemberCode={6}&PlayDate={7}",
                                sessionInfo.GoodsCode, sessionInfo.PlaceCode, sessionInfo.OnlyDeliver, sessionInfo.DBDay, sessionInfo.ExpressDelyDay, sessionInfo.BizCode, sessionInfo.BizMemberCode, sessionInfo.PlayDate));
                httpWRequest.Method = "get";
                httpWRequest.Referer = string.Format(
                                "http://ticket.interpark.com/Book/BookDatetime.asp?GoodsCode={0}&PlaceCode={1}&OnlyDeliver={2}&DBDay={3}&ExpressDelyDay={4}&Tiki={5}&BizCode={6}&BizMemberCode={7}&KindOfGoods={8}&Always={9}&HotSaleOrNot={10}&PlayDate=&PlaySeq=&SessionId={11}",
                                sessionInfo.GoodsCode, sessionInfo.PlaceCode, sessionInfo.OnlyDeliver, sessionInfo.DBDay, sessionInfo.ExpressDelyDay, sessionInfo.Tiki, sessionInfo.BizCode, sessionInfo.BizMemberCode, sessionInfo.KindOfGoods, sessionInfo.Always, sessionInfo.HotSaleOrNot, sessionInfo.SessionId);
                httpWRequest.CookieContainer = new CookieContainer();

                Util.SetCookie(cookie, httpWRequest);

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream());

                string resultHtml = sr.ReadToEnd();
                var valueSearch = resultHtml;
                foreach (var propertyInfo in typeof(SessionInfo).GetProperties())
                {

                    if (valueSearch.IndexOf("<" + propertyInfo.Name + ">") > -1)
                    {
                        var value = valueSearch.Substring("<" + propertyInfo.Name + ">", "</");

                        var property = sessionInfo.GetType().GetProperty(propertyInfo.Name);
                        if (property.GetValue(sessionInfo, null) == null || (string) property.GetValue(sessionInfo, null) == "")
                        {
                            property.SetValue(sessionInfo, value);
                        }
                    }
                }

                var interparkstamp = theResponse.Headers.GetValues("Set-Cookie").FirstOrDefault(x => x.Contains("interparkstamp"));
                interparkstamp = interparkstamp.Substring("interparkstamp=", ";");

                var searchText = cookie.Substring("interparkstamp=", ";");
                cookie.Replace(searchText, interparkstamp);

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 2-2. 기초 정보 조회
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <param name="cookie"></param>
        public static void GetBaseXmlData2(ref SessionInfo sessionInfo, ref string cookie)
        {
            try
            {
                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create(string.Format(
                                "http://ticket.interpark.com/Book/Lib/BookInfoXml.asp?Flag=PlaySeqOthers&GoodsCode={0}&PlaceCode={1}&PlaySeq={2}",
                                sessionInfo.GoodsCode, sessionInfo.PlaceCode, sessionInfo.PlaySeq));
                httpWRequest.Method = "get";
                httpWRequest.Referer = "http://ticket.interpark.com/Book/BookMain.asp";
                httpWRequest.CookieContainer = new CookieContainer();

                Util.SetCookie(cookie, httpWRequest);

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream());

                string resultHtml = sr.ReadToEnd();
                var valueSearch = resultHtml;
                foreach (var propertyInfo in typeof(SessionInfo).GetProperties())
                {

                    if (valueSearch.IndexOf("<" + propertyInfo.Name + ">") > -1)
                    {
                        var value = valueSearch.Substring("<" + propertyInfo.Name + ">", "</");

                        var property = sessionInfo.GetType().GetProperty(propertyInfo.Name);
                        if (property.GetValue(sessionInfo, null) == null || (string)property.GetValue(sessionInfo, null) == "")
                        {
                            property.SetValue(sessionInfo, value);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        ///  티켓정보 조회
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static void GetTicketInfo(ref SessionInfo sessionInfo, ref string cookie)
        {
            try
            {
                string parameter =
                   string.Format(
                       "Tiki={0}&TikiGrade={1}&TikiDiscountCnt={2}&BizCode={3}&GoodsBizCode={4}&GoodsBizName={5}&BizMemberCode={6}&SessionId={7}&BadUserOrNot={8}&TMemHash={9}&SIDBizCode={10}&TicketAmt={11}&TicketCnt={12}&TotalAmt={13}&TotalUseAmt={14}&DelyAmt={15}&VoucherAmt={16}&GroupCode={17}&GoodsCode={18}&PlaceCode={19}&OnlyDeliver={20}&DBDay={21}&ExpressDelyDay={22}&DelyFee={23}&ExpressDelyFee={24}&ReservedOrNot={25}&Always={26}&GroupName={27}&PlaceName={28}&LocOfImage={29}&TmgsOrNot={30}&KindOfGoods={31}&SubCategory={32}&GoodsOption={33}&UseAmt={34}&TypeOfUse={35}&PenaltyScript={36}&SupplyType={37}&RcptTaxYN={38}&HotSaleOrNot={39}&AgreeOrNot={40}&UserInfoOrNot={41}&SeasonOrNot={42}&StartDate={43}&EndDate={44}&PackageOrNot={45}&MobileOrNot={46}&HomeOrNot={47}&SecretGoodsYN={48}&LuckyBagGoodsYN={49}&PlayDate={50}&PlaySeq={51}&PlayTime={52}&NoOfTime={53}&OnlineDate={54}&CancelableDate={55}&BookingEndDate={56}&LmtKindOfSettle={57}&LmtCardIssueCode={58}&LmtSalesYN={59}&KindOfDiscount={60}&DiscountName={61}&DiscountAmt={62}&DiscountSeq={63}&TikiKindOfDiscount={64}&OtherKindOfDiscount={65}&CardKindOfDiscount={66}&CardKind={67}&CardDiscountKind={68}&CardIssueCode={69}&CardDiscountNo={70}&CardDelivery={71}&Point={72}&PointSeq={73}&PointDiscountAmt={74}&CouponCode={75}&CouponName={76}&Coupon_DiscountValue={77}&Coupon_TypeOfPay={78}&Coupon_ChargeInfo={79}&Coupon_DblDiscountOrNot={80}&Coupon_DiscountAmt={81}&DeliveryOrNot={82}&DeliveryGift={83}&DeliveryGiftAmt={84}&Delivery={85}&RName={86}&RPhoneNo={87}&RHpNo={88}&RZipcode={89}&RAddr={90}&RSubAddr={91}&MemberName={92}&SSN={93}&PhoneNo={94}&HpNo={95}&Email={96}&SmsOrNot={97}&Zipcode={98}&Addr={99}&SubAddr={100}&DeliveryEnc={101}&MCash_TelKind={102}&MCash_No={103}&MCash_SSN={104}&MCash_Smsval={105}&MCash_Phoneid={106}&MCash_Traid={107}&MCash_UseAmt={108}&MCash_TotalAmt={109}&KindOfSettle={110}&CartID={111}&CartIDSeq={112}&DoubleChecked={113}&PaymentDouble={114}&TotGiftSttledAmt={115}&ISPDiscountOrNot={116}&DiscountOrNot={117}&TicketCntTikiDiscount={118}&FirstKindOfPayment={119}&SecondKindOfPayment={120}&FirstSettleAmt={121}&SecondSettleAmt={122}&BankCode={123}&KindOfCard={124}&DiscountCard={125}&CardNo={126}&ValidInfo={127}&CardSSN={128}&CardPWD={129}&HalbuMonth={130}&LGPoint={131}&WooriPoint={132}&CitiPoint={133}&KBPoint={134}&MPoint={135}&SSPoint={136}&KEBPoint={137}&HashPrice={138}&CultrueUser={139}&HappyUser={140}&FirstSubKindOfPayment={141}&PaymentEnc={142}&PayDiscountCode={143}&Encrypted={144}&RN={145}&IspCardCode={146}&CardCode={147}&CardQuota={148}&BearsNumber={149}&GSPwd={150}&GSCardNo={151}&GSType={152}&GSPointUseYN={153}&GSPointSaveYN={154}&useGSPoint={155}&GSPointCheckYN={156}&OtherSiteGSCardNo={157}&OtherSiteGSYN={158}&OtherSiteGSPwd={159}&expoRecommander={160}&expoRecOrg={161}&expoName={162}&expoImg={163}&FCSeasonNo={164}",
                       sessionInfo.Tiki.UrlDecode(),sessionInfo.TikiGrade.UrlDecode(),sessionInfo.TikiDiscountCnt.UrlDecode(),sessionInfo.BizCode.UrlDecode(),sessionInfo.GoodsBizCode.UrlDecode(),sessionInfo.GoodsBizName.UrlDecode(),sessionInfo.BizMemberCode.UrlDecode(),sessionInfo.SessionId.UrlDecode(),sessionInfo.BadUserOrNot.UrlDecode(),sessionInfo.TMemHash.UrlDecode(),sessionInfo.SIDBizCode.UrlDecode(),sessionInfo.TicketAmt.UrlDecode(),sessionInfo.TicketCnt.UrlDecode(),sessionInfo.TotalAmt.UrlDecode(),sessionInfo.TotalUseAmt.UrlDecode(),sessionInfo.DelyAmt.UrlDecode(),sessionInfo.VoucherAmt.UrlDecode(),sessionInfo.GroupCode.UrlDecode(),sessionInfo.GoodsCode.UrlDecode(),sessionInfo.PlaceCode.UrlDecode(),sessionInfo.OnlyDeliver.UrlDecode(),sessionInfo.DBDay.UrlDecode(),sessionInfo.ExpressDelyDay.UrlDecode(),sessionInfo.DelyFee.UrlDecode(),sessionInfo.ExpressDelyFee.UrlDecode(),sessionInfo.ReservedOrNot.UrlDecode(),sessionInfo.Always.UrlDecode(),sessionInfo.GroupName.UrlDecode(),sessionInfo.PlaceName.UrlDecode(),sessionInfo.LocOfImage.UrlDecode(),sessionInfo.TmgsOrNot.UrlDecode(),sessionInfo.KindOfGoods.UrlDecode(),sessionInfo.SubCategory.UrlDecode(),sessionInfo.GoodsOption.UrlDecode(),sessionInfo.UseAmt.UrlDecode(),sessionInfo.TypeOfUse.UrlDecode(),sessionInfo.PenaltyScript.UrlDecode(),sessionInfo.SupplyType.UrlDecode(),sessionInfo.RcptTaxYN.UrlDecode(),sessionInfo.HotSaleOrNot.UrlDecode(),sessionInfo.AgreeOrNot.UrlDecode(),sessionInfo.UserInfoOrNot.UrlDecode(),sessionInfo.SeasonOrNot.UrlDecode(),sessionInfo.StartDate.UrlDecode(),sessionInfo.EndDate.UrlDecode(),sessionInfo.PackageOrNot.UrlDecode(),sessionInfo.MobileOrNot.UrlDecode(),sessionInfo.HomeOrNot.UrlDecode(),sessionInfo.SecretGoodsYN.UrlDecode(),sessionInfo.LuckyBagGoodsYN.UrlDecode(),sessionInfo.PlayDate.UrlDecode(),sessionInfo.PlaySeq.UrlDecode(),sessionInfo.PlayTime.UrlDecode(),sessionInfo.NoOfTime.UrlDecode(),sessionInfo.OnlineDate.UrlDecode(),sessionInfo.CancelableDate.UrlDecode(),sessionInfo.BookingEndDate.UrlDecode(),sessionInfo.LmtKindOfSettle.UrlDecode(),sessionInfo.LmtCardIssueCode.UrlDecode(),sessionInfo.LmtSalesYN.UrlDecode(),sessionInfo.KindOfDiscount.UrlDecode(),sessionInfo.DiscountName.UrlDecode(),sessionInfo.DiscountAmt.UrlDecode(),sessionInfo.DiscountSeq.UrlDecode(),sessionInfo.TikiKindOfDiscount.UrlDecode(),sessionInfo.OtherKindOfDiscount.UrlDecode(),sessionInfo.CardKindOfDiscount.UrlDecode(),sessionInfo.CardKind.UrlDecode(),sessionInfo.CardDiscountKind.UrlDecode(),sessionInfo.CardIssueCode.UrlDecode(),sessionInfo.CardDiscountNo.UrlDecode(),sessionInfo.CardDelivery.UrlDecode(),sessionInfo.Point.UrlDecode(),sessionInfo.PointSeq.UrlDecode(),sessionInfo.PointDiscountAmt.UrlDecode(),sessionInfo.CouponCode.UrlDecode(),sessionInfo.CouponName.UrlDecode(),sessionInfo.Coupon_DiscountValue.UrlDecode(),sessionInfo.Coupon_TypeOfPay.UrlDecode(),sessionInfo.Coupon_ChargeInfo.UrlDecode(),sessionInfo.Coupon_DblDiscountOrNot.UrlDecode(),sessionInfo.Coupon_DiscountAmt.UrlDecode(),sessionInfo.DeliveryOrNot.UrlDecode(),sessionInfo.DeliveryGift.UrlDecode(),sessionInfo.DeliveryGiftAmt.UrlDecode(),sessionInfo.Delivery.UrlDecode(),sessionInfo.RName.UrlDecode(),sessionInfo.RPhoneNo.UrlDecode(),sessionInfo.RHpNo.UrlDecode(),sessionInfo.RZipcode.UrlDecode(),sessionInfo.RAddr.UrlDecode(),sessionInfo.RSubAddr.UrlDecode(),sessionInfo.MemberName.UrlDecode(),sessionInfo.SSN.UrlDecode(),sessionInfo.PhoneNo.UrlDecode(),sessionInfo.HpNo.UrlDecode(),sessionInfo.Email.UrlDecode(),sessionInfo.SmsOrNot.UrlDecode(),sessionInfo.Zipcode.UrlDecode(),sessionInfo.Addr.UrlDecode(),sessionInfo.SubAddr.UrlDecode(),sessionInfo.DeliveryEnc.UrlDecode(),sessionInfo.MCash_TelKind.UrlDecode(),sessionInfo.MCash_No.UrlDecode(),sessionInfo.MCash_SSN.UrlDecode(),sessionInfo.MCash_Smsval.UrlDecode(),sessionInfo.MCash_Phoneid.UrlDecode(),sessionInfo.MCash_Traid.UrlDecode(),sessionInfo.MCash_UseAmt.UrlDecode(),sessionInfo.MCash_TotalAmt.UrlDecode(),sessionInfo.KindOfSettle.UrlDecode(),sessionInfo.CartID.UrlDecode(),sessionInfo.CartIDSeq.UrlDecode(),sessionInfo.DoubleChecked.UrlDecode(),sessionInfo.PaymentDouble.UrlDecode(),"0",sessionInfo.ISPDiscountOrNot.UrlDecode(),sessionInfo.DiscountOrNot.UrlDecode(),sessionInfo.TicketCntTikiDiscount.UrlDecode(),sessionInfo.FirstKindOfPayment.UrlDecode(),sessionInfo.SecondKindOfPayment.UrlDecode(),"0","0",sessionInfo.BankCode.UrlDecode(),sessionInfo.KindOfCard.UrlDecode(),sessionInfo.DiscountCard.UrlDecode(),sessionInfo.CardNo.UrlDecode(),sessionInfo.ValidInfo.UrlDecode(),sessionInfo.CardSSN.UrlDecode(),sessionInfo.CardPWD.UrlDecode(),sessionInfo.HalbuMonth.UrlDecode(),sessionInfo.LGPoint.UrlDecode(),sessionInfo.WooriPoint.UrlDecode(),sessionInfo.CitiPoint.UrlDecode(),sessionInfo.KBPoint.UrlDecode(),sessionInfo.MPoint.UrlDecode(),sessionInfo.SSPoint.UrlDecode(),sessionInfo.KEBPoint.UrlDecode(),sessionInfo.HashPrice.UrlDecode(),sessionInfo.CultrueUser.UrlDecode(),sessionInfo.HappyUser.UrlDecode(),sessionInfo.FirstSubKindOfPayment.UrlDecode(),sessionInfo.PaymentEnc.UrlDecode(),sessionInfo.PayDiscountCode.UrlDecode(),sessionInfo.Encrypted.UrlDecode(),sessionInfo.RN.UrlDecode(),sessionInfo.IspCardCode.UrlDecode(),sessionInfo.CardCode.UrlDecode(),sessionInfo.CardQuota.UrlDecode(),sessionInfo.BearsNumber.UrlDecode(),sessionInfo.GSPwd.UrlDecode(),sessionInfo.GSCardNo.UrlDecode(),sessionInfo.GSType.UrlDecode(),sessionInfo.GSPointUseYN.UrlDecode(),sessionInfo.GSPointSaveYN.UrlDecode(),sessionInfo.useGSPoint.UrlDecode(),sessionInfo.GSPointCheckYN.UrlDecode(),sessionInfo.OtherSiteGSCardNo.UrlDecode(),sessionInfo.OtherSiteGSYN.UrlDecode(),sessionInfo.OtherSiteGSPwd.UrlDecode(),sessionInfo.expoRecommander.UrlDecode(),sessionInfo.expoRecOrg.UrlDecode(),sessionInfo.expoName.UrlDecode(),sessionInfo.expoImg.UrlDecode(),sessionInfo.FCSeasonNo.UrlDecode());
                
                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create("http://ticket.interpark.com/Book/BookPrice.asp");
                httpWRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.";
                httpWRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                httpWRequest.Headers.Add("Accept-Language", "ko-KR");
                httpWRequest.Headers.Add("Origin", "http://ticket.interpark.com");
                httpWRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
                httpWRequest.Referer = "http://ticket.interpark.com/Book/BookMain.asp";
                httpWRequest.Headers.Add("DNT", "1");
                httpWRequest.ContentType = "application/x-www-form-urlencoded;";
                httpWRequest.KeepAlive = true;
                httpWRequest.Method = "Post";
                httpWRequest.ContentLength = parameter.Length;
                httpWRequest.CookieContainer = new CookieContainer();
                httpWRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Util.SetCookie(cookie, httpWRequest);

                var sw = new StreamWriter(httpWRequest.GetRequestStream());
                sw.Write(parameter);
                sw.Close();

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                string resultHtml = sr.ReadToEnd();

                var valueSearch =
                    resultHtml.Substring(
                        "<tr id=\"PriceRow000\">",
                        "</tr>");

                foreach (var propertyInfo in typeof(SessionInfo).GetProperties())
                {

                    if (valueSearch.IndexOf(propertyInfo.Name + "=") > -1)
                    {
                        var value = valueSearch.Substring(propertyInfo.Name + "=", "\n").Replace("\"","");

                        var property = sessionInfo.GetType().GetProperty(propertyInfo.Name);
                        if (property.GetValue(sessionInfo, null) == null)
                        {
                            if (property.PropertyType.ToString().Contains("List"))
                            {
                                var listValue = new List<string>();
                                for (int i = 0; i < 2; i++)
                                {
                                    listValue.Add(value);    
                                }

                                property.SetValue(sessionInfo, listValue);    
                            }
                            else
                            {
                                property.SetValue(sessionInfo, value);    
                            }
                        }
                    }
                }

                var interparkstamp = theResponse.Headers.GetValues("Set-Cookie").FirstOrDefault(x => x.Contains("interparkstamp"));
                interparkstamp = interparkstamp.Substring("interparkstamp=", ";");

                var searchText = cookie.Substring("interparkstamp=", ";");
                cookie.Replace(searchText, interparkstamp);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 유저 정보 조회
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool GetUserInfo(ref SessionInfo sessionInfo, ref string cookie)
        {
            try
            {
                
                string parameter =
                   string.Format(
                       "Tiki={0}&TikiGrade={1}&TikiDiscountCnt={2}&BizCode={3}&GoodsBizCode={4}&GoodsBizName={5}&BizMemberCode={6}&SessionId={7}&BadUserOrNot={8}&TMemHash={9}&SIDBizCode={10}&TicketAmt={11}&TicketCnt={12}&TotalAmt={13}&TotalUseAmt={14}&DelyAmt={15}&VoucherAmt={16}&GroupCode={17}&GoodsCode={18}&PlaceCode={19}&OnlyDeliver={20}&DBDay={21}&ExpressDelyDay={22}&DelyFee={23}&ExpressDelyFee={24}&ReservedOrNot={25}&Always={26}&GroupName={27}&PlaceName={28}&LocOfImage={29}&TmgsOrNot={30}&KindOfGoods={31}&SubCategory={32}&GoodsOption={33}&UseAmt={34}&TypeOfUse={35}&PenaltyScript={36}&SupplyType={37}&RcptTaxYN={38}&HotSaleOrNot={39}&AgreeOrNot={40}&UserInfoOrNot={41}&SeasonOrNot={42}&StartDate={43}&EndDate={44}&PackageOrNot={45}&MobileOrNot={46}&HomeOrNot={47}&SecretGoodsYN={48}&LuckyBagGoodsYN={49}&PlayDate={50}&PlaySeq={51}&PlayTime={52}&NoOfTime={53}&OnlineDate={54}&CancelableDate={55}&BookingEndDate={56}&LmtKindOfSettle={57}&LmtCardIssueCode={58}&LmtSalesYN={59}&KindOfDiscount={60}&DiscountName={61}&DiscountAmt={62}&DiscountSeq={63}&TikiKindOfDiscount={64}&OtherKindOfDiscount={65}&CardKindOfDiscount={66}&CardKind={67}&CardDiscountKind={68}&CardIssueCode={69}&CardDiscountNo={70}&CardDelivery={71}&Point={72}&PointSeq={73}&PointDiscountAmt={74}&CouponCode={75}&CouponName={76}&Coupon_DiscountValue={77}&Coupon_TypeOfPay={78}&Coupon_ChargeInfo={79}&Coupon_DblDiscountOrNot={80}&Coupon_DiscountAmt={81}&DeliveryOrNot={82}&DeliveryGift={83}&DeliveryGiftAmt={84}&Delivery={85}&RName={86}&RPhoneNo={87}&RHpNo={88}&RZipcode={89}&RAddr={90}&RSubAddr={91}&MemberName={92}&SSN={93}&PhoneNo={94}&HpNo={95}&Email={96}&SmsOrNot={97}&Zipcode={98}&Addr={99}&SubAddr={100}&DeliveryEnc={101}&MCash_TelKind={102}&MCash_No={103}&MCash_SSN={104}&MCash_Smsval={105}&MCash_Phoneid={106}&MCash_Traid={107}&MCash_UseAmt={108}&MCash_TotalAmt={109}&KindOfSettle={110}&CartID={111}&CartIDSeq={112}&DoubleChecked={113}&PaymentDouble={114}&TotGiftSttledAmt={115}&ISPDiscountOrNot={116}&DiscountOrNot={117}&TicketCntTikiDiscount={118}&FirstKindOfPayment={119}&SecondKindOfPayment={120}&FirstSettleAmt={121}&SecondSettleAmt={122}&BankCode={123}&KindOfCard={124}&DiscountCard={125}&CardNo={126}&ValidInfo={127}&CardSSN={128}&CardPWD={129}&HalbuMonth={130}&LGPoint={131}&WooriPoint={132}&CitiPoint={133}&KBPoint={134}&MPoint={135}&SSPoint={136}&KEBPoint={137}&HashPrice={138}&CultrueUser={139}&HappyUser={140}&FirstSubKindOfPayment={141}&PaymentEnc={142}&PayDiscountCode={143}&Encrypted={144}&RN={145}&IspCardCode={146}&CardCode={147}&CardQuota={148}&BearsNumber={149}&GSPwd={150}&GSCardNo={151}&GSType={152}&GSPointUseYN={153}&GSPointSaveYN={154}&useGSPoint={155}&GSPointCheckYN={156}&OtherSiteGSCardNo={157}&OtherSiteGSYN={158}&OtherSiteGSPwd={159}&expoRecommander={160}&expoRecOrg={161}&expoName={162}&expoImg={163}&FCSeasonNo={164}&PGType={165}&SeatGrade={166}&PriceGrade={167}&SalesPrice={168}&DblDiscountOrNot={169}&GroupId={170}&SeatGradeName={171}&PriceGradeName={172}&BlockNo={173}&Floor={174}&RowNo={175}&SeatNo={176}&VoucherCode={177}&VoucherSalesPrice={178}&DiscountCode={179}&SeatInfo={180}&PGType={181}&SeatGrade={182}&PriceGrade={183}&SalesPrice={184}&DblDiscountOrNot={185}&GroupId={186}&SeatGradeName={187}&PriceGradeName={188}&BlockNo={189}&Floor={190}&RowNo={191}&SeatNo={192}&VoucherCode={193}&VoucherSalesPrice={194}&DiscountCode={195}&SeatInfo={196}",
                       sessionInfo.Tiki.UrlDecode(),sessionInfo.TikiGrade.UrlDecode(),sessionInfo.TikiDiscountCnt.UrlDecode(),sessionInfo.BizCode.UrlDecode(),sessionInfo.GoodsBizCode.UrlDecode(),sessionInfo.GoodsBizName.UrlDecode(),sessionInfo.BizMemberCode.UrlDecode(),sessionInfo.SessionId.UrlDecode(),sessionInfo.BadUserOrNot.UrlDecode(),sessionInfo.TMemHash.UrlDecode(),sessionInfo.SIDBizCode.UrlDecode(),sessionInfo.TicketAmt.UrlDecode(),sessionInfo.TicketCnt.UrlDecode(),sessionInfo.TotalAmt.UrlDecode(),sessionInfo.TotalUseAmt.UrlDecode(),sessionInfo.DelyAmt.UrlDecode(),sessionInfo.VoucherAmt.UrlDecode(),sessionInfo.GroupCode.UrlDecode(),sessionInfo.GoodsCode.UrlDecode(),sessionInfo.PlaceCode.UrlDecode(),sessionInfo.OnlyDeliver.UrlDecode(),sessionInfo.DBDay.UrlDecode(),sessionInfo.ExpressDelyDay.UrlDecode(),sessionInfo.DelyFee.UrlDecode(),sessionInfo.ExpressDelyFee.UrlDecode(),sessionInfo.ReservedOrNot.UrlDecode(),sessionInfo.Always.UrlDecode(),sessionInfo.GroupName.UrlDecode(),sessionInfo.PlaceName.UrlDecode(),sessionInfo.LocOfImage.UrlDecode(),sessionInfo.TmgsOrNot.UrlDecode(),sessionInfo.KindOfGoods.UrlDecode(),sessionInfo.SubCategory.UrlDecode(),sessionInfo.GoodsOption.UrlDecode(),sessionInfo.UseAmt.UrlDecode(),sessionInfo.TypeOfUse.UrlDecode(),sessionInfo.PenaltyScript.UrlDecode(),sessionInfo.SupplyType.UrlDecode(),sessionInfo.RcptTaxYN.UrlDecode(),sessionInfo.HotSaleOrNot.UrlDecode(),sessionInfo.AgreeOrNot.UrlDecode(),sessionInfo.UserInfoOrNot.UrlDecode(),sessionInfo.SeasonOrNot.UrlDecode(),sessionInfo.StartDate.UrlDecode(),sessionInfo.EndDate.UrlDecode(),sessionInfo.PackageOrNot.UrlDecode(),sessionInfo.MobileOrNot.UrlDecode(),sessionInfo.HomeOrNot.UrlDecode(),sessionInfo.SecretGoodsYN.UrlDecode(),sessionInfo.LuckyBagGoodsYN.UrlDecode(),sessionInfo.PlayDate.UrlDecode(),sessionInfo.PlaySeq.UrlDecode(),sessionInfo.PlayTime.UrlDecode(),sessionInfo.NoOfTime.UrlDecode(),sessionInfo.OnlineDate.UrlDecode(),sessionInfo.CancelableDate.UrlDecode(),sessionInfo.BookingEndDate.UrlDecode(),sessionInfo.LmtKindOfSettle.UrlDecode(),sessionInfo.LmtCardIssueCode.UrlDecode(),sessionInfo.LmtSalesYN.UrlDecode(),sessionInfo.KindOfDiscount.UrlDecode(),sessionInfo.DiscountName.UrlDecode(),sessionInfo.DiscountAmt.UrlDecode(),sessionInfo.DiscountSeq.UrlDecode(),sessionInfo.TikiKindOfDiscount.UrlDecode(),sessionInfo.OtherKindOfDiscount.UrlDecode(),sessionInfo.CardKindOfDiscount.UrlDecode(),sessionInfo.CardKind.UrlDecode(),sessionInfo.CardDiscountKind.UrlDecode(),sessionInfo.CardIssueCode.UrlDecode(),sessionInfo.CardDiscountNo.UrlDecode(),sessionInfo.CardDelivery.UrlDecode(),sessionInfo.Point.UrlDecode(),sessionInfo.PointSeq.UrlDecode(),sessionInfo.PointDiscountAmt.UrlDecode(),sessionInfo.CouponCode.UrlDecode(),sessionInfo.CouponName.UrlDecode(),sessionInfo.Coupon_DiscountValue.UrlDecode(),sessionInfo.Coupon_TypeOfPay.UrlDecode(),sessionInfo.Coupon_ChargeInfo.UrlDecode(),sessionInfo.Coupon_DblDiscountOrNot.UrlDecode(),sessionInfo.Coupon_DiscountAmt.UrlDecode(),sessionInfo.DeliveryOrNot.UrlDecode(),sessionInfo.DeliveryGift.UrlDecode(),sessionInfo.DeliveryGiftAmt.UrlDecode(),sessionInfo.Delivery.UrlDecode(),sessionInfo.RName.UrlDecode(),sessionInfo.RPhoneNo.UrlDecode(),sessionInfo.RHpNo.UrlDecode(),sessionInfo.RZipcode.UrlDecode(),sessionInfo.RAddr.UrlDecode(),sessionInfo.RSubAddr.UrlDecode(),sessionInfo.MemberName.UrlDecode(),sessionInfo.SSN.UrlDecode(),sessionInfo.PhoneNo.UrlDecode(),sessionInfo.HpNo.UrlDecode(),sessionInfo.Email.UrlDecode(),sessionInfo.SmsOrNot.UrlDecode(),sessionInfo.Zipcode.UrlDecode(),sessionInfo.Addr.UrlDecode(),sessionInfo.SubAddr.UrlDecode(),sessionInfo.DeliveryEnc.UrlDecode(),sessionInfo.MCash_TelKind.UrlDecode(),sessionInfo.MCash_No.UrlDecode(),sessionInfo.MCash_SSN.UrlDecode(),sessionInfo.MCash_Smsval.UrlDecode(),sessionInfo.MCash_Phoneid.UrlDecode(),sessionInfo.MCash_Traid.UrlDecode(),sessionInfo.MCash_UseAmt.UrlDecode(),sessionInfo.MCash_TotalAmt.UrlDecode(),sessionInfo.KindOfSettle.UrlDecode(),sessionInfo.CartID.UrlDecode(),sessionInfo.CartIDSeq.UrlDecode(),sessionInfo.DoubleChecked.UrlDecode(),sessionInfo.PaymentDouble.UrlDecode(),sessionInfo.TotGiftSttledAmt.UrlDecode(),sessionInfo.ISPDiscountOrNot.UrlDecode(),sessionInfo.DiscountOrNot.UrlDecode(),sessionInfo.TicketCntTikiDiscount.UrlDecode(),sessionInfo.FirstKindOfPayment.UrlDecode(),sessionInfo.SecondKindOfPayment.UrlDecode(),sessionInfo.FirstSettleAmt.UrlDecode(),sessionInfo.SecondSettleAmt.UrlDecode(),sessionInfo.BankCode.UrlDecode(),sessionInfo.KindOfCard.UrlDecode(),sessionInfo.DiscountCard.UrlDecode(),sessionInfo.CardNo.UrlDecode(),sessionInfo.ValidInfo.UrlDecode(),sessionInfo.CardSSN.UrlDecode(),sessionInfo.CardPWD.UrlDecode(),sessionInfo.HalbuMonth.UrlDecode(),sessionInfo.LGPoint.UrlDecode(),sessionInfo.WooriPoint.UrlDecode(),sessionInfo.CitiPoint.UrlDecode(),sessionInfo.KBPoint.UrlDecode(),sessionInfo.MPoint.UrlDecode(),sessionInfo.SSPoint.UrlDecode(),sessionInfo.KEBPoint.UrlDecode(),sessionInfo.HashPrice.UrlDecode(),sessionInfo.CultrueUser.UrlDecode(),sessionInfo.HappyUser.UrlDecode(),sessionInfo.FirstSubKindOfPayment.UrlDecode(),sessionInfo.PaymentEnc.UrlDecode(),sessionInfo.PayDiscountCode.UrlDecode(),sessionInfo.Encrypted.UrlDecode(),sessionInfo.RN.UrlDecode(),sessionInfo.IspCardCode.UrlDecode(),sessionInfo.CardCode.UrlDecode(),sessionInfo.CardQuota.UrlDecode(),sessionInfo.BearsNumber.UrlDecode(),sessionInfo.GSPwd.UrlDecode(),sessionInfo.GSCardNo.UrlDecode(),sessionInfo.GSType.UrlDecode(),sessionInfo.GSPointUseYN.UrlDecode(),sessionInfo.GSPointSaveYN.UrlDecode(),sessionInfo.useGSPoint.UrlDecode(),sessionInfo.GSPointCheckYN.UrlDecode(),sessionInfo.OtherSiteGSCardNo.UrlDecode(),sessionInfo.OtherSiteGSYN.UrlDecode(),sessionInfo.OtherSiteGSPwd.UrlDecode(),sessionInfo.expoRecommander.UrlDecode(),sessionInfo.expoRecOrg.UrlDecode(),sessionInfo.expoName.UrlDecode(),sessionInfo.expoImg.UrlDecode(),sessionInfo.FCSeasonNo.UrlDecode(),sessionInfo.PGType.UrlDecode(),sessionInfo.SeatGrade.UrlDecode(),sessionInfo.PriceGrade.UrlDecode(),sessionInfo.SalesPrice.UrlDecode(),sessionInfo.DblDiscountOrNot.UrlDecode(),sessionInfo.GroupId.UrlDecode(),sessionInfo.SeatGradeName.UrlDecode(),sessionInfo.PriceGradeName.UrlDecode(),sessionInfo.BlockNo.UrlDecode(),sessionInfo.Floor.UrlDecode(),sessionInfo.RowNo.UrlDecode(),sessionInfo.SeatNo.UrlDecode(),sessionInfo.VoucherCode.UrlDecode(),sessionInfo.VoucherSalesPrice.UrlDecode(),sessionInfo.DiscountCode.UrlDecode(),sessionInfo.SeatInfo.UrlDecode(),sessionInfo.PGType.UrlDecode(),sessionInfo.SeatGrade.UrlDecode(),sessionInfo.PriceGrade.UrlDecode(),sessionInfo.SalesPrice.UrlDecode(),sessionInfo.DblDiscountOrNot.UrlDecode(),sessionInfo.GroupId.UrlDecode(),sessionInfo.SeatGradeName.UrlDecode(),sessionInfo.PriceGradeName.UrlDecode(),sessionInfo.BlockNo.UrlDecode(),sessionInfo.Floor.UrlDecode(),sessionInfo.RowNo.UrlDecode(),sessionInfo.SeatNo.UrlDecode(),sessionInfo.VoucherCode.UrlDecode(),sessionInfo.VoucherSalesPrice.UrlDecode(),sessionInfo.DiscountCode.UrlDecode(),sessionInfo.SeatInfo.UrlDecode());
                
                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create("http://ticket.interpark.com/Book/BookDelivery.asp");
                httpWRequest.Accept = "text/html, application/xhtml+xml, */*";
                httpWRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                httpWRequest.Headers.Add("Accept-Language", "ko-KR");
                httpWRequest.Referer = "http://ticket.interpark.com/Book/BookMain.asp";
                httpWRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
                httpWRequest.Headers.Add("DNT", "1");
                httpWRequest.ContentType = "application/x-www-form-urlencoded;";
                httpWRequest.KeepAlive = true;
                httpWRequest.Method = "Post";
                httpWRequest.ContentLength = parameter.Length;
                httpWRequest.CookieContainer = new CookieContainer();
                httpWRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Util.SetCookie(cookie, httpWRequest);

                var sw = new StreamWriter(httpWRequest.GetRequestStream(), Encoding.GetEncoding("euc-kr"));
                sw.Write(parameter);
                sw.Close();

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                string resultHtml = sr.ReadToEnd();

                if (resultHtml.Contains("예매정보가 올바르지 않습니다"))
                {
                    return false;
                }

                sessionInfo.SSN = resultHtml.Substring("if (SSN.substring(0, 6) != \"", "\"){") + "1" + "000000";

                sessionInfo.PhoneNo = string.Format("{0}-{1}-{2}",
                    resultHtml.Substring("id=\"PhoneNo1\" value=\"", "\" s"),
                    resultHtml.Substring("id=\"PhoneNo2\" value=\"", "\" s"),
                    resultHtml.Substring("id=\"PhoneNo3\" value=\"", "\" s"));
                sessionInfo.RPhoneNo = sessionInfo.PhoneNo;

                sessionInfo.HpNo = string.Format("{0}-{1}-{2}",
                    resultHtml.Substring("id=\"HpNo1\" value=\"", "\" s"),
                    resultHtml.Substring("id=\"HpNo2\" value=\"", "\" s"),
                    resultHtml.Substring("id=\"HpNo3\" value=\"", "\" s"));
                sessionInfo.RHpNo = sessionInfo.HpNo;

                sessionInfo.Zipcode = string.Format("{0}-{1}",
                    resultHtml.Substring("id=\"Zipcode1\" value=\"", "\">"),
                    resultHtml.Substring("id=\"Zipcode2\" value=\"", "\">"));
                sessionInfo.RZipcode = sessionInfo.Zipcode;

                var valueSearch = resultHtml;

                foreach (var propertyInfo in typeof(SessionInfo).GetProperties())
                {
                    if (valueSearch.IndexOf("id=\"" + propertyInfo.Name + "\" value=\"") <= -1) continue;

                    var value = valueSearch.Substring("id=\"" + propertyInfo.Name + "\" value=\"", "\">");

                    if (propertyInfo.Name == "Email")
                    {
                        value = valueSearch.Substring("id=\"" + propertyInfo.Name + "\" value=\"", "\" s");
                    }

                    var property = sessionInfo.GetType().GetProperty(propertyInfo.Name);
                    if (property.GetValue(sessionInfo, null) == null)
                    {
                        property.SetValue(sessionInfo, value);
                    }
                }

                sessionInfo.RAddr = sessionInfo.Addr;
                sessionInfo.RSubAddr = sessionInfo.SubAddr;
                sessionInfo.RName = sessionInfo.MemberName;
                sessionInfo.DeliveryOrNot = "Y";
                sessionInfo.Delivery = "24001"; // 배송 2,500
                sessionInfo.SmsOrNot = "Y";
                sessionInfo.DeliveryEnc = "Y";
                sessionInfo.KindOfDiscount = "";

                var priceTotal = 0;
                foreach (var price in sessionInfo.SalesPrice)
                {
                    priceTotal +=int.Parse(price);    
                }

                sessionInfo.TicketAmt = priceTotal.ToString();
                sessionInfo.TicketCnt = "3";
                sessionInfo.TotalUseAmt = (1000 * int.Parse(sessionInfo.TicketCnt)).ToString();
                sessionInfo.DelyAmt = "2500";
                sessionInfo.DiscountAmt = sessionInfo.TicketAmt;
                sessionInfo.TotalAmt = (int.Parse(sessionInfo.TicketAmt) + int.Parse(sessionInfo.TotalUseAmt) + int.Parse(sessionInfo.DelyAmt)).ToString();
                sessionInfo.PaymentEnc = "Y";
                sessionInfo.OtherSiteGSYN = "Y";


                var interparkstamp = theResponse.Headers.GetValues("Set-Cookie").FirstOrDefault(x => x.Contains("interparkstamp"));
                interparkstamp = interparkstamp.Substring("interparkstamp=", ";");

                var searchText = cookie.Substring("interparkstamp=", ";");
                cookie.Replace(searchText, interparkstamp);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 취소수수료
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static void GetPanaltyScript(ref SessionInfo session)
        {
            try
            {
                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create(string.Format(
                                "http://ticket.interpark.com/Book/Lib/BookInfo.asp?Flag=CPanalty&BizCode={0}&GoodsCode={1}&PlaceCode={2}&PlayDate={3}&CancelableDate={4}&PlaySeq={5}&PackageOrNot=N&GoodsName={6}",
                                session.BizCode,
                                session.GoodsCode,
                                session.PlaceCode,
                                session.PlayDate,
                                session.CancelableDate,
                                session.PlaySeq,
                                session.GoodsBizName
                                ));
                httpWRequest.Method = "get";
                httpWRequest.Referer = "http://ticket.interpark.com/Book/BookMain.asp";

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                var resultHtml = sr.ReadToEnd();
                session.PenaltyScript = resultHtml.Replace("\r\n<script type=\"text/javascript\" src=\"/Book/Inc/Js/common.js\"></script>\r\n","");
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 암호화 데이터
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static void GetEncryptData(ref SessionInfo session, ref string cookie)
        {
            try
            {
                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create(string.Format(
                                "https://ticket.interpark.com/Book/Lib/BookEncrypt.asp?SSN={0}&PhoneNo={1}&HpNo={2}&RPhoneNo={1}&RHpNo={2}&Email={3}&Callback=fnDeliveryEncryptCallback",
                                session.SSN,
                                session.PhoneNo,
                                session.HpNo,
                                session.Email
                                ));
                httpWRequest.Method = "get";
                httpWRequest.Referer = "http://ticket.interpark.com/Book/BookDelivery.asp";
                httpWRequest.CookieContainer = new CookieContainer();
                httpWRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Util.SetCookie(cookie, httpWRequest);


                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                var resultHtml = sr.ReadToEnd();
                session.SSN = resultHtml.Substring("SSN\":\"", "\",");
                session.PhoneNo = resultHtml.Substring("PhoneNo\":\"", "\",");
                session.HpNo = resultHtml.Substring("HpNo\":\"", "\",");
                session.RPhoneNo = resultHtml.Substring("RPhoneNo\":\"", "\",");
                session.RHpNo = resultHtml.Substring("RHpNo\":\"", "\",");
                session.Email = resultHtml.Substring("Email\":\"", "\"}");
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 암호화 데이터
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static void GetMCacheEncryptData(ref SessionInfo session, ref string cookie)
        {
            try
            {
                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create(string.Format(
                                "https://ticket.interpark.com/Book/Lib/BookEncrypt.asp?MCash_No=&MCash_SSN=&CardNo=&ValidInfo=&CardSSN=&CardPWD=&MCashTotalAmt={0}&Callback=fnPaymentEncryptCallback",
                                session.TotalAmt
                                ));
                httpWRequest.Method = "get";
                httpWRequest.Referer = "http://ticket.interpark.com/Book/BookDelivery.asp";
                httpWRequest.CookieContainer = new CookieContainer();
                httpWRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Util.SetCookie(cookie, httpWRequest);


                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                var resultHtml = sr.ReadToEnd();
                session.MCash_TotalAmt = resultHtml.Substring("MCash_TotalAmt\":\"", "\"}");
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 결재 정보 조회
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static void GetPayment(ref SessionInfo sessionInfo, ref string cookie)
        {
            try
            {

                string parameter =
                   string.Format(
                       "Tiki={0}&TikiGrade={1}&TikiDiscountCnt={2}&BizCode={3}&GoodsBizCode={4}&GoodsBizName={5}&BizMemberCode={6}&SessionId={7}&BadUserOrNot={8}&TMemHash={9}&SIDBizCode={10}&TicketAmt={11}&TicketCnt={12}&TotalAmt={13}&TotalUseAmt={14}&DelyAmt={15}&VoucherAmt={16}&GroupCode={17}&GoodsCode={18}&PlaceCode={19}&OnlyDeliver={20}&DBDay={21}&ExpressDelyDay={22}&DelyFee={23}&ExpressDelyFee={24}&ReservedOrNot={25}&Always={26}&GroupName={27}&PlaceName={28}&LocOfImage={29}&TmgsOrNot={30}&KindOfGoods={31}&SubCategory={32}&GoodsOption={33}&UseAmt={34}&TypeOfUse={35}&PenaltyScript={36}&SupplyType={37}&RcptTaxYN={38}&HotSaleOrNot={39}&AgreeOrNot={40}&UserInfoOrNot={41}&SeasonOrNot={42}&StartDate={43}&EndDate={44}&PackageOrNot={45}&MobileOrNot={46}&HomeOrNot={47}&SecretGoodsYN={48}&LuckyBagGoodsYN={49}&PlayDate={50}&PlaySeq={51}&PlayTime={52}&NoOfTime={53}&OnlineDate={54}&CancelableDate={55}&BookingEndDate={56}&LmtKindOfSettle={57}&LmtCardIssueCode={58}&LmtSalesYN={59}&KindOfDiscount={60}&DiscountName={61}&DiscountAmt=198000&DiscountSeq=&TikiKindOfDiscount={64}&OtherKindOfDiscount={65}&CardKindOfDiscount={66}&CardKind={67}&CardDiscountKind={68}&CardIssueCode={69}&CardDiscountNo={70}&CardDelivery={71}&Point={72}&PointSeq={73}&PointDiscountAmt={74}&CouponCode={75}&CouponName={76}&Coupon_DiscountValue={77}&Coupon_TypeOfPay={78}&Coupon_ChargeInfo={79}&Coupon_DblDiscountOrNot={80}&Coupon_DiscountAmt={81}&DeliveryOrNot={82}&DeliveryGift={83}&DeliveryGiftAmt={84}&Delivery={85}&RName={86}&RPhoneNo={87}&RHpNo={88}&RZipcode={89}&RAddr={90}&RSubAddr={91}&MemberName={92}&SSN={93}&PhoneNo={94}&HpNo={95}&Email={96}&SmsOrNot={97}&Zipcode={98}&Addr={99}&SubAddr={100}&DeliveryEnc={101}&MCash_TelKind={102}&MCash_No={103}&MCash_SSN={104}&MCash_Smsval={105}&MCash_Phoneid={106}&MCash_Traid={107}&MCash_UseAmt={108}&MCash_TotalAmt={109}&KindOfSettle={110}&CartID={111}&CartIDSeq={112}&DoubleChecked={113}&PaymentDouble={114}&TotGiftSttledAmt={115}&ISPDiscountOrNot={116}&DiscountOrNot={117}&TicketCntTikiDiscount={118}&FirstKindOfPayment={119}&SecondKindOfPayment={120}&FirstSettleAmt={121}&SecondSettleAmt={122}&BankCode={123}&KindOfCard={124}&DiscountCard={125}&CardNo={126}&ValidInfo={127}&CardSSN={128}&CardPWD={129}&HalbuMonth={130}&LGPoint={131}&WooriPoint={132}&CitiPoint={133}&KBPoint={134}&MPoint={135}&SSPoint={136}&KEBPoint={137}&HashPrice={138}&CultrueUser={139}&HappyUser={140}&FirstSubKindOfPayment={141}&PaymentEnc={142}&PayDiscountCode={143}&Encrypted={144}&RN={145}&IspCardCode={146}&CardCode={147}&CardQuota={148}&BearsNumber={149}&GSPwd={150}&GSCardNo={151}&GSType={152}&GSPointUseYN={153}&GSPointSaveYN={154}&useGSPoint={155}&GSPointCheckYN={156}&OtherSiteGSCardNo={157}&OtherSiteGSYN={158}&OtherSiteGSPwd={159}&expoRecommander={160}&expoRecOrg={161}&expoName={162}&expoImg={163}&FCSeasonNo={164}&PGType={165}&SeatGrade={166}&PriceGrade={167}&SalesPrice={168}&DblDiscountOrNot={169}&GroupId={170}&SeatGradeName={171}&PriceGradeName={172}&BlockNo={173}&Floor={174}&RowNo={175}&SeatNo={176}&VoucherCode={177}&VoucherSalesPrice={178}&DiscountCode={179}&SeatInfo={180}&PGType={181}&SeatGrade={182}&PriceGrade={183}&SalesPrice={184}&DblDiscountOrNot={185}&GroupId={186}&SeatGradeName={187}&PriceGradeName={188}&BlockNo={189}&Floor={190}&RowNo={191}&SeatNo={192}&VoucherCode={193}&VoucherSalesPrice={194}&DiscountCode={195}&SeatInfo={196}",
                       sessionInfo.Tiki.UrlDecode(),sessionInfo.TikiGrade.UrlDecode(),sessionInfo.TikiDiscountCnt.UrlDecode(),sessionInfo.BizCode.UrlDecode(),sessionInfo.GoodsBizCode.UrlDecode(),sessionInfo.GoodsBizName.UrlDecode(),sessionInfo.BizMemberCode.UrlDecode(),sessionInfo.SessionId.UrlDecode(),sessionInfo.BadUserOrNot.UrlDecode(),sessionInfo.TMemHash.UrlDecode(),sessionInfo.SIDBizCode.UrlDecode(),sessionInfo.TicketAmt.UrlDecode(),sessionInfo.TicketCnt.UrlDecode(),sessionInfo.TotalAmt.UrlDecode(),sessionInfo.TotalUseAmt.UrlDecode(),sessionInfo.DelyAmt.UrlDecode(),sessionInfo.VoucherAmt.UrlDecode(),sessionInfo.GroupCode.UrlDecode(),sessionInfo.GoodsCode.UrlDecode(),sessionInfo.PlaceCode.UrlDecode(),sessionInfo.OnlyDeliver.UrlDecode(),sessionInfo.DBDay.UrlDecode(),sessionInfo.ExpressDelyDay.UrlDecode(),sessionInfo.DelyFee.UrlDecode(),sessionInfo.ExpressDelyFee.UrlDecode(),sessionInfo.ReservedOrNot.UrlDecode(),sessionInfo.Always.UrlDecode(),sessionInfo.GroupName.UrlDecode(),sessionInfo.PlaceName.UrlDecode(),sessionInfo.LocOfImage.UrlDecode(),sessionInfo.TmgsOrNot.UrlDecode(),sessionInfo.KindOfGoods.UrlDecode(),sessionInfo.SubCategory.UrlDecode(),sessionInfo.GoodsOption.UrlDecode(),sessionInfo.UseAmt.UrlDecode(),sessionInfo.TypeOfUse.UrlDecode(),sessionInfo.PenaltyScript.UrlDecode(),sessionInfo.SupplyType.UrlDecode(),sessionInfo.RcptTaxYN.UrlDecode(),sessionInfo.HotSaleOrNot.UrlDecode(),sessionInfo.AgreeOrNot.UrlDecode(),sessionInfo.UserInfoOrNot.UrlDecode(),sessionInfo.SeasonOrNot.UrlDecode(),sessionInfo.StartDate.UrlDecode(),sessionInfo.EndDate.UrlDecode(),sessionInfo.PackageOrNot.UrlDecode(),sessionInfo.MobileOrNot.UrlDecode(),sessionInfo.HomeOrNot.UrlDecode(),sessionInfo.SecretGoodsYN.UrlDecode(),sessionInfo.LuckyBagGoodsYN.UrlDecode(),sessionInfo.PlayDate.UrlDecode(),sessionInfo.PlaySeq.UrlDecode(),sessionInfo.PlayTime.UrlDecode(),sessionInfo.NoOfTime.UrlDecode(),sessionInfo.OnlineDate.UrlDecode(),sessionInfo.CancelableDate.UrlDecode(),sessionInfo.BookingEndDate.UrlDecode(),sessionInfo.LmtKindOfSettle.UrlDecode(),sessionInfo.LmtCardIssueCode.UrlDecode(),sessionInfo.LmtSalesYN.UrlDecode(),sessionInfo.KindOfDiscount.UrlDecode(),sessionInfo.DiscountName.UrlDecode(),sessionInfo.DiscountAmt.UrlDecode(),sessionInfo.DiscountSeq.UrlDecode(),sessionInfo.TikiKindOfDiscount.UrlDecode(),sessionInfo.OtherKindOfDiscount.UrlDecode(),sessionInfo.CardKindOfDiscount.UrlDecode(),sessionInfo.CardKind.UrlDecode(),sessionInfo.CardDiscountKind.UrlDecode(),sessionInfo.CardIssueCode.UrlDecode(),sessionInfo.CardDiscountNo.UrlDecode(),sessionInfo.CardDelivery.UrlDecode(),sessionInfo.Point.UrlDecode(),sessionInfo.PointSeq.UrlDecode(),sessionInfo.PointDiscountAmt.UrlDecode(),sessionInfo.CouponCode.UrlDecode(),sessionInfo.CouponName.UrlDecode(),sessionInfo.Coupon_DiscountValue.UrlDecode(),sessionInfo.Coupon_TypeOfPay.UrlDecode(),sessionInfo.Coupon_ChargeInfo.UrlDecode(),sessionInfo.Coupon_DblDiscountOrNot.UrlDecode(),sessionInfo.Coupon_DiscountAmt.UrlDecode(),sessionInfo.DeliveryOrNot.UrlDecode(),sessionInfo.DeliveryGift.UrlDecode(),sessionInfo.DeliveryGiftAmt.UrlDecode(),sessionInfo.Delivery.UrlDecode(),sessionInfo.RName.UrlDecode(),sessionInfo.RPhoneNo.UrlDecode(),sessionInfo.RHpNo.UrlDecode(),sessionInfo.RZipcode.UrlDecode(),sessionInfo.RAddr.UrlDecode(),sessionInfo.RSubAddr.UrlDecode(),sessionInfo.MemberName.UrlDecode(),sessionInfo.SSN.UrlDecode(),sessionInfo.PhoneNo.UrlDecode(),sessionInfo.HpNo.UrlDecode(),sessionInfo.Email.UrlDecode(),sessionInfo.SmsOrNot.UrlDecode(),sessionInfo.Zipcode.UrlDecode(),sessionInfo.Addr.UrlDecode(),sessionInfo.SubAddr.UrlDecode(),sessionInfo.DeliveryEnc.UrlDecode(),sessionInfo.MCash_TelKind.UrlDecode(),sessionInfo.MCash_No.UrlDecode(),sessionInfo.MCash_SSN.UrlDecode(),sessionInfo.MCash_Smsval.UrlDecode(),sessionInfo.MCash_Phoneid.UrlDecode(),sessionInfo.MCash_Traid.UrlDecode(),sessionInfo.MCash_UseAmt.UrlDecode(),sessionInfo.MCash_TotalAmt.UrlDecode(),sessionInfo.KindOfSettle.UrlDecode(),sessionInfo.CartID.UrlDecode(),sessionInfo.CartIDSeq.UrlDecode(),sessionInfo.DoubleChecked.UrlDecode(),sessionInfo.PaymentDouble.UrlDecode(),sessionInfo.TotGiftSttledAmt.UrlDecode(),sessionInfo.ISPDiscountOrNot.UrlDecode(),sessionInfo.DiscountOrNot.UrlDecode(),sessionInfo.TicketCntTikiDiscount.UrlDecode(),sessionInfo.FirstKindOfPayment.UrlDecode(),sessionInfo.SecondKindOfPayment.UrlDecode(),sessionInfo.FirstSettleAmt.UrlDecode(),sessionInfo.SecondSettleAmt.UrlDecode(),sessionInfo.BankCode.UrlDecode(),sessionInfo.KindOfCard.UrlDecode(),sessionInfo.DiscountCard.UrlDecode(),sessionInfo.CardNo.UrlDecode(),sessionInfo.ValidInfo.UrlDecode(),sessionInfo.CardSSN.UrlDecode(),sessionInfo.CardPWD.UrlDecode(),sessionInfo.HalbuMonth.UrlDecode(),sessionInfo.LGPoint.UrlDecode(),sessionInfo.WooriPoint.UrlDecode(),sessionInfo.CitiPoint.UrlDecode(),sessionInfo.KBPoint.UrlDecode(),sessionInfo.MPoint.UrlDecode(),sessionInfo.SSPoint.UrlDecode(),sessionInfo.KEBPoint.UrlDecode(),sessionInfo.HashPrice.UrlDecode(),sessionInfo.CultrueUser.UrlDecode(),sessionInfo.HappyUser.UrlDecode(),sessionInfo.FirstSubKindOfPayment.UrlDecode(),sessionInfo.PaymentEnc.UrlDecode(),sessionInfo.PayDiscountCode.UrlDecode(),sessionInfo.Encrypted.UrlDecode(),sessionInfo.RN.UrlDecode(),sessionInfo.IspCardCode.UrlDecode(),sessionInfo.CardCode.UrlDecode(),sessionInfo.CardQuota.UrlDecode(),sessionInfo.BearsNumber.UrlDecode(),sessionInfo.GSPwd.UrlDecode(),sessionInfo.GSCardNo.UrlDecode(),sessionInfo.GSType.UrlDecode(),sessionInfo.GSPointUseYN.UrlDecode(),sessionInfo.GSPointSaveYN.UrlDecode(),sessionInfo.useGSPoint.UrlDecode(),sessionInfo.GSPointCheckYN.UrlDecode(),sessionInfo.OtherSiteGSCardNo.UrlDecode(),sessionInfo.OtherSiteGSYN.UrlDecode(),sessionInfo.OtherSiteGSPwd.UrlDecode(),sessionInfo.expoRecommander.UrlDecode(),sessionInfo.expoRecOrg.UrlDecode(),sessionInfo.expoName.UrlDecode(),sessionInfo.expoImg.UrlDecode(),sessionInfo.FCSeasonNo.UrlDecode(),sessionInfo.PGType.UrlDecode(),sessionInfo.SeatGrade.UrlDecode(),sessionInfo.PriceGrade.UrlDecode(),sessionInfo.SalesPrice.UrlDecode(),sessionInfo.DblDiscountOrNot.UrlDecode(),sessionInfo.GroupId.UrlDecode(),sessionInfo.SeatGradeName.UrlDecode(),sessionInfo.PriceGradeName.UrlDecode(),sessionInfo.BlockNo.UrlDecode(),sessionInfo.Floor.UrlDecode(),sessionInfo.RowNo.UrlDecode(),sessionInfo.SeatNo.UrlDecode(),sessionInfo.VoucherCode.UrlDecode(),sessionInfo.VoucherSalesPrice.UrlDecode(),sessionInfo.DiscountCode.UrlDecode(),sessionInfo.SeatInfo.UrlDecode(),sessionInfo.PGType.UrlDecode(),sessionInfo.SeatGrade.UrlDecode(),sessionInfo.PriceGrade.UrlDecode(),sessionInfo.SalesPrice.UrlDecode(),sessionInfo.DblDiscountOrNot.UrlDecode(),sessionInfo.GroupId.UrlDecode(),sessionInfo.SeatGradeName.UrlDecode(),sessionInfo.PriceGradeName.UrlDecode(),sessionInfo.BlockNo.UrlDecode(),sessionInfo.Floor.UrlDecode(),sessionInfo.RowNo.UrlDecode(),sessionInfo.SeatNo.UrlDecode(),sessionInfo.VoucherCode.UrlDecode(),sessionInfo.VoucherSalesPrice.UrlDecode(),sessionInfo.DiscountCode.UrlDecode(),sessionInfo.SeatInfo.UrlDecode());

                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create("http://ticket.interpark.com/Book/BookPayment.asp");
                httpWRequest.Accept = "text/html, application/xhtml+xml, */*";
                httpWRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                httpWRequest.Headers.Add("Accept-Language", "ko-KR");
                httpWRequest.Referer = "http://ticket.interpark.com/Book/BookMain.asp";
                httpWRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
                httpWRequest.Headers.Add("DNT", "1");
                httpWRequest.ContentType = "application/x-www-form-urlencoded;";
                httpWRequest.KeepAlive = true;
                httpWRequest.Method = "Post";
                httpWRequest.ContentLength = parameter.Length;
                httpWRequest.CookieContainer = new CookieContainer();
                httpWRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Util.SetCookie(cookie, httpWRequest);

                var sw = new StreamWriter(httpWRequest.GetRequestStream(), Encoding.GetEncoding("euc-kr"));
                sw.Write(parameter);
                sw.Close();

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                string resultHtml = sr.ReadToEnd();


                sessionInfo.FirstKindOfPayment = "22004";// 무통장입금
                sessionInfo.FirstSettleAmt = sessionInfo.TotalAmt;
                sessionInfo.BankCode = "38052"; // 국민은행
                sessionInfo.HashPrice = resultHtml.Substring("$F(\"PointOject\"), \"", "\", $F(");

                var interparkstamp = theResponse.Headers.GetValues("Set-Cookie").FirstOrDefault(x => x.Contains("interparkstamp"));
                interparkstamp = interparkstamp.Substring("interparkstamp=", ";");

                var searchText = cookie.Substring("interparkstamp=", ";");
                cookie.Replace(searchText, interparkstamp);
           
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 결재동의
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static void Confirm(ref SessionInfo sessionInfo, ref string cookie)
        {
            try
            {

                string parameter =
                   string.Format(
                       "Tiki={0}&TikiGrade={1}&TikiDiscountCnt={2}&BizCode={3}&GoodsBizCode={4}&GoodsBizName={5}&BizMemberCode={6}&SessionId={7}&BadUserOrNot={8}&TMemHash={9}&SIDBizCode={10}&TicketAmt={11}&TicketCnt={12}&TotalAmt={13}&TotalUseAmt={14}&DelyAmt={15}&VoucherAmt={16}&GroupCode={17}&GoodsCode={18}&PlaceCode={19}&OnlyDeliver={20}&DBDay={21}&ExpressDelyDay={22}&DelyFee={23}&ExpressDelyFee={24}&ReservedOrNot={25}&Always={26}&GroupName={27}&PlaceName={28}&LocOfImage={29}&TmgsOrNot={30}&KindOfGoods={31}&SubCategory={32}&GoodsOption={33}&UseAmt={34}&TypeOfUse={35}&PenaltyScript={36}&SupplyType={37}&RcptTaxYN={38}&HotSaleOrNot={39}&AgreeOrNot={40}&UserInfoOrNot={41}&SeasonOrNot={42}&StartDate={43}&EndDate={44}&PackageOrNot={45}&MobileOrNot={46}&HomeOrNot={47}&SecretGoodsYN={48}&LuckyBagGoodsYN={49}&PlayDate={50}&PlaySeq={51}&PlayTime={52}&NoOfTime={53}&OnlineDate={54}&CancelableDate={55}&BookingEndDate={56}&LmtKindOfSettle={57}&LmtCardIssueCode={58}&LmtSalesYN={59}&KindOfDiscount={60}&DiscountName={61}&DiscountAmt={62}&DiscountSeq={63}&TikiKindOfDiscount={64}&OtherKindOfDiscount={65}&CardKindOfDiscount={66}&CardKind={67}&CardDiscountKind={68}&CardIssueCode={69}&CardDiscountNo={70}&CardDelivery={71}&Point={72}&PointSeq={73}&PointDiscountAmt={74}&CouponCode={75}&CouponName={76}&Coupon_DiscountValue={77}&Coupon_TypeOfPay={78}&Coupon_ChargeInfo={79}&Coupon_DblDiscountOrNot={80}&Coupon_DiscountAmt={81}&DeliveryOrNot={82}&DeliveryGift={83}&DeliveryGiftAmt={84}&Delivery={85}&RName={86}&RPhoneNo={87}&RHpNo={88}&RZipcode={89}&RAddr={90}&RSubAddr={91}&MemberName={92}&SSN={93}&PhoneNo={94}&HpNo={95}&Email={96}&SmsOrNot={97}&Zipcode={98}&Addr={99}&SubAddr={100}&DeliveryEnc={101}&MCash_TelKind={102}&MCash_No={103}&MCash_SSN={104}&MCash_Smsval={105}&MCash_Phoneid={106}&MCash_Traid={107}&MCash_UseAmt={108}&MCash_TotalAmt={109}&KindOfSettle={110}&CartID={111}&CartIDSeq={112}&DoubleChecked={113}&PaymentDouble={114}&TotGiftSttledAmt={115}&ISPDiscountOrNot={116}&DiscountOrNot={117}&TicketCntTikiDiscount={118}&FirstKindOfPayment={119}&SecondKindOfPayment={120}&FirstSettleAmt={121}&SecondSettleAmt={122}&BankCode={123}&KindOfCard={124}&DiscountCard={125}&CardNo={126}&ValidInfo={127}&CardSSN={128}&CardPWD={129}&HalbuMonth={130}&LGPoint={131}&WooriPoint={132}&CitiPoint={133}&KBPoint={134}&MPoint={135}&SSPoint={136}&KEBPoint={137}&HashPrice={138}&CultrueUser={139}&HappyUser={140}&FirstSubKindOfPayment={141}&PaymentEnc={142}&PayDiscountCode={143}&Encrypted={144}&RN={145}&IspCardCode={146}&CardCode={147}&CardQuota={148}&BearsNumber={149}&GSPwd={150}&GSCardNo={151}&GSType={152}&GSPointUseYN={153}&GSPointSaveYN={154}&useGSPoint={155}&GSPointCheckYN={156}&OtherSiteGSCardNo={157}&OtherSiteGSYN={158}&OtherSiteGSPwd={159}&expoRecommander={160}&expoRecOrg={161}&expoName={162}&expoImg={163}&FCSeasonNo={164}&PGType={165}&SeatGrade={166}&PriceGrade={167}&SalesPrice={168}&DblDiscountOrNot={169}&GroupId={170}&SeatGradeName={171}&PriceGradeName={172}&BlockNo={173}&Floor={174}&RowNo={175}&SeatNo={176}&VoucherCode={177}&VoucherSalesPrice={178}&DiscountCode={179}&SeatInfo={180}&PGType={181}&SeatGrade={182}&PriceGrade={183}&SalesPrice={184}&DblDiscountOrNot={185}&GroupId={186}&SeatGradeName={187}&PriceGradeName={188}&BlockNo={189}&Floor={190}&RowNo={191}&SeatNo={192}&VoucherCode={193}&VoucherSalesPrice={194}&DiscountCode={195}&SeatInfo={196}",
                       sessionInfo.Tiki.UrlDecode(),sessionInfo.TikiGrade.UrlDecode(),sessionInfo.TikiDiscountCnt.UrlDecode(),sessionInfo.BizCode.UrlDecode(),sessionInfo.GoodsBizCode.UrlDecode(),sessionInfo.GoodsBizName.UrlDecode(),sessionInfo.BizMemberCode.UrlDecode(),sessionInfo.SessionId.UrlDecode(),sessionInfo.BadUserOrNot.UrlDecode(),sessionInfo.TMemHash.UrlDecode(),sessionInfo.SIDBizCode.UrlDecode(),sessionInfo.TicketAmt.UrlDecode(),sessionInfo.TicketCnt.UrlDecode(),sessionInfo.TotalAmt.UrlDecode(),sessionInfo.TotalUseAmt.UrlDecode(),sessionInfo.DelyAmt.UrlDecode(),sessionInfo.VoucherAmt.UrlDecode(),sessionInfo.GroupCode.UrlDecode(),sessionInfo.GoodsCode.UrlDecode(),sessionInfo.PlaceCode.UrlDecode(),sessionInfo.OnlyDeliver.UrlDecode(),sessionInfo.DBDay.UrlDecode(),sessionInfo.ExpressDelyDay.UrlDecode(),sessionInfo.DelyFee.UrlDecode(),sessionInfo.ExpressDelyFee.UrlDecode(),sessionInfo.ReservedOrNot.UrlDecode(),sessionInfo.Always.UrlDecode(),sessionInfo.GroupName.UrlDecode(),sessionInfo.PlaceName.UrlDecode(),sessionInfo.LocOfImage.UrlDecode(),sessionInfo.TmgsOrNot.UrlDecode(),sessionInfo.KindOfGoods.UrlDecode(),sessionInfo.SubCategory.UrlDecode(),sessionInfo.GoodsOption.UrlDecode(),sessionInfo.UseAmt.UrlDecode(),sessionInfo.TypeOfUse.UrlDecode(),sessionInfo.PenaltyScript.UrlDecode(),sessionInfo.SupplyType.UrlDecode(),sessionInfo.RcptTaxYN.UrlDecode(),sessionInfo.HotSaleOrNot.UrlDecode(),sessionInfo.AgreeOrNot.UrlDecode(),sessionInfo.UserInfoOrNot.UrlDecode(),sessionInfo.SeasonOrNot.UrlDecode(),sessionInfo.StartDate.UrlDecode(),sessionInfo.EndDate.UrlDecode(),sessionInfo.PackageOrNot.UrlDecode(),sessionInfo.MobileOrNot.UrlDecode(),sessionInfo.HomeOrNot.UrlDecode(),sessionInfo.SecretGoodsYN.UrlDecode(),sessionInfo.LuckyBagGoodsYN.UrlDecode(),sessionInfo.PlayDate.UrlDecode(),sessionInfo.PlaySeq.UrlDecode(),sessionInfo.PlayTime.UrlDecode(),sessionInfo.NoOfTime.UrlDecode(),sessionInfo.OnlineDate.UrlDecode(),sessionInfo.CancelableDate.UrlDecode(),sessionInfo.BookingEndDate.UrlDecode(),sessionInfo.LmtKindOfSettle.UrlDecode(),sessionInfo.LmtCardIssueCode.UrlDecode(),sessionInfo.LmtSalesYN.UrlDecode(),sessionInfo.KindOfDiscount.UrlDecode(),sessionInfo.DiscountName.UrlDecode(),sessionInfo.DiscountAmt.UrlDecode(),sessionInfo.DiscountSeq.UrlDecode(),sessionInfo.TikiKindOfDiscount.UrlDecode(),sessionInfo.OtherKindOfDiscount.UrlDecode(),sessionInfo.CardKindOfDiscount.UrlDecode(),sessionInfo.CardKind.UrlDecode(),sessionInfo.CardDiscountKind.UrlDecode(),sessionInfo.CardIssueCode.UrlDecode(),sessionInfo.CardDiscountNo.UrlDecode(),sessionInfo.CardDelivery.UrlDecode(),sessionInfo.Point.UrlDecode(),sessionInfo.PointSeq.UrlDecode(),sessionInfo.PointDiscountAmt.UrlDecode(),sessionInfo.CouponCode.UrlDecode(),sessionInfo.CouponName.UrlDecode(),sessionInfo.Coupon_DiscountValue.UrlDecode(),sessionInfo.Coupon_TypeOfPay.UrlDecode(),sessionInfo.Coupon_ChargeInfo.UrlDecode(),sessionInfo.Coupon_DblDiscountOrNot.UrlDecode(),sessionInfo.Coupon_DiscountAmt.UrlDecode(),sessionInfo.DeliveryOrNot.UrlDecode(),sessionInfo.DeliveryGift.UrlDecode(),sessionInfo.DeliveryGiftAmt.UrlDecode(),sessionInfo.Delivery.UrlDecode(),sessionInfo.RName.UrlDecode(),sessionInfo.RPhoneNo.UrlDecode(),sessionInfo.RHpNo.UrlDecode(),sessionInfo.RZipcode.UrlDecode(),sessionInfo.RAddr.UrlDecode(),sessionInfo.RSubAddr.UrlDecode(),sessionInfo.MemberName.UrlDecode(),sessionInfo.SSN.UrlDecode(),sessionInfo.PhoneNo.UrlDecode(),sessionInfo.HpNo.UrlDecode(),sessionInfo.Email.UrlDecode(),sessionInfo.SmsOrNot.UrlDecode(),sessionInfo.Zipcode.UrlDecode(),sessionInfo.Addr.UrlDecode(),sessionInfo.SubAddr.UrlDecode(),sessionInfo.DeliveryEnc.UrlDecode(),sessionInfo.MCash_TelKind.UrlDecode(),sessionInfo.MCash_No.UrlDecode(),sessionInfo.MCash_SSN.UrlDecode(),sessionInfo.MCash_Smsval.UrlDecode(),sessionInfo.MCash_Phoneid.UrlDecode(),sessionInfo.MCash_Traid.UrlDecode(),sessionInfo.MCash_UseAmt.UrlDecode(),sessionInfo.MCash_TotalAmt.UrlDecode(),sessionInfo.KindOfSettle.UrlDecode(),sessionInfo.CartID.UrlDecode(),sessionInfo.CartIDSeq.UrlDecode(),sessionInfo.DoubleChecked.UrlDecode(),sessionInfo.PaymentDouble.UrlDecode(),sessionInfo.TotGiftSttledAmt.UrlDecode(),sessionInfo.ISPDiscountOrNot.UrlDecode(),sessionInfo.DiscountOrNot.UrlDecode(),sessionInfo.TicketCntTikiDiscount.UrlDecode(),sessionInfo.FirstKindOfPayment.UrlDecode(),sessionInfo.SecondKindOfPayment.UrlDecode(),sessionInfo.FirstSettleAmt.UrlDecode(),sessionInfo.SecondSettleAmt.UrlDecode(),sessionInfo.BankCode.UrlDecode(),sessionInfo.KindOfCard.UrlDecode(),sessionInfo.DiscountCard.UrlDecode(),sessionInfo.CardNo.UrlDecode(),sessionInfo.ValidInfo.UrlDecode(),sessionInfo.CardSSN.UrlDecode(),sessionInfo.CardPWD.UrlDecode(),sessionInfo.HalbuMonth.UrlDecode(),sessionInfo.LGPoint.UrlDecode(),sessionInfo.WooriPoint.UrlDecode(),sessionInfo.CitiPoint.UrlDecode(),sessionInfo.KBPoint.UrlDecode(),sessionInfo.MPoint.UrlDecode(),sessionInfo.SSPoint.UrlDecode(),sessionInfo.KEBPoint.UrlDecode(),sessionInfo.HashPrice.UrlDecode(),sessionInfo.CultrueUser.UrlDecode(),sessionInfo.HappyUser.UrlDecode(),sessionInfo.FirstSubKindOfPayment.UrlDecode(),sessionInfo.PaymentEnc.UrlDecode(),sessionInfo.PayDiscountCode.UrlDecode(),sessionInfo.Encrypted.UrlDecode(),sessionInfo.RN.UrlDecode(),sessionInfo.IspCardCode.UrlDecode(),sessionInfo.CardCode.UrlDecode(),sessionInfo.CardQuota.UrlDecode(),sessionInfo.BearsNumber.UrlDecode(),sessionInfo.GSPwd.UrlDecode(),sessionInfo.GSCardNo.UrlDecode(),sessionInfo.GSType.UrlDecode(),sessionInfo.GSPointUseYN.UrlDecode(),sessionInfo.GSPointSaveYN.UrlDecode(),sessionInfo.useGSPoint.UrlDecode(),sessionInfo.GSPointCheckYN.UrlDecode(),sessionInfo.OtherSiteGSCardNo.UrlDecode(),sessionInfo.OtherSiteGSYN.UrlDecode(),sessionInfo.OtherSiteGSPwd.UrlDecode(),sessionInfo.expoRecommander.UrlDecode(),sessionInfo.expoRecOrg.UrlDecode(),sessionInfo.expoName.UrlDecode(),sessionInfo.expoImg.UrlDecode(),sessionInfo.FCSeasonNo.UrlDecode(),sessionInfo.PGType.UrlDecode(),sessionInfo.SeatGrade.UrlDecode(),sessionInfo.PriceGrade.UrlDecode(),sessionInfo.SalesPrice.UrlDecode(),sessionInfo.DblDiscountOrNot.UrlDecode(),sessionInfo.GroupId.UrlDecode(),sessionInfo.SeatGradeName.UrlDecode(),sessionInfo.PriceGradeName.UrlDecode(),sessionInfo.BlockNo.UrlDecode(),sessionInfo.Floor.UrlDecode(),sessionInfo.RowNo.UrlDecode(),sessionInfo.SeatNo.UrlDecode(),sessionInfo.VoucherCode.UrlDecode(),sessionInfo.VoucherSalesPrice.UrlDecode(),sessionInfo.DiscountCode.UrlDecode(),sessionInfo.SeatInfo.UrlDecode(),sessionInfo.PGType.UrlDecode(),sessionInfo.SeatGrade.UrlDecode(),sessionInfo.PriceGrade.UrlDecode(),sessionInfo.SalesPrice.UrlDecode(),sessionInfo.DblDiscountOrNot.UrlDecode(),sessionInfo.GroupId.UrlDecode(),sessionInfo.SeatGradeName.UrlDecode(),sessionInfo.PriceGradeName.UrlDecode(),sessionInfo.BlockNo.UrlDecode(),sessionInfo.Floor.UrlDecode(),sessionInfo.RowNo.UrlDecode(),sessionInfo.SeatNo.UrlDecode(),sessionInfo.VoucherCode.UrlDecode(),sessionInfo.VoucherSalesPrice.UrlDecode(),sessionInfo.DiscountCode.UrlDecode(),sessionInfo.SeatInfo.UrlDecode());

                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create("http://ticket.interpark.com/Book/BookConfirm.asp");
                httpWRequest.Accept = "text/html, application/xhtml+xml, */*";
                httpWRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                httpWRequest.Headers.Add("Accept-Language", "ko-KR");
                httpWRequest.Referer = "http://ticket.interpark.com/Book/BookMain.asp";
                httpWRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
                httpWRequest.Headers.Add("DNT", "1");
                httpWRequest.ContentType = "application/x-www-form-urlencoded;";
                httpWRequest.KeepAlive = true;
                httpWRequest.Method = "Post";
                httpWRequest.ContentLength = parameter.Length;
                httpWRequest.CookieContainer = new CookieContainer();
                httpWRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Util.SetCookie(cookie, httpWRequest);

                var sw = new StreamWriter(httpWRequest.GetRequestStream(), Encoding.GetEncoding("euc-kr"));
                sw.Write(parameter);
                sw.Close();

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                string resultHtml = sr.ReadToEnd();

                var splitData = resultHtml.Substring("parent.fnSetBookConfirm('", "','','',").Split(',');
                sessionInfo.CartID = splitData[0].Replace("'", "");
                sessionInfo.CartIDSeq = splitData[1].Replace("'", "");

                var interparkstamp = theResponse.Headers.GetValues("Set-Cookie").FirstOrDefault(x => x.Contains("interparkstamp"));
                interparkstamp = interparkstamp.Substring("interparkstamp=", ";");

                var searchText = cookie.Substring("interparkstamp=", ";");
                cookie.Replace(searchText, interparkstamp);

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 결재
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool End(ref SessionInfo sessionInfo, ref string cookie)
        {
            try
            {

                string parameter =
                   string.Format(
                       "Tiki={0}&TikiGrade={1}&TikiDiscountCnt={2}&BizCode={3}&GoodsBizCode={4}&GoodsBizName={5}&BizMemberCode={6}&SessionId={7}&BadUserOrNot={8}&TMemHash={9}&SIDBizCode={10}&TicketAmt={11}&TicketCnt={12}&TotalAmt={13}&TotalUseAmt={14}&DelyAmt={15}&VoucherAmt={16}&GroupCode={17}&GoodsCode={18}&PlaceCode={19}&OnlyDeliver={20}&DBDay={21}&ExpressDelyDay={22}&DelyFee={23}&ExpressDelyFee={24}&ReservedOrNot={25}&Always={26}&GroupName={27}&PlaceName={28}&LocOfImage={29}&TmgsOrNot={30}&KindOfGoods={31}&SubCategory={32}&GoodsOption={33}&UseAmt={34}&TypeOfUse={35}&PenaltyScript={36}&SupplyType={37}&RcptTaxYN={38}&HotSaleOrNot={39}&AgreeOrNot={40}&UserInfoOrNot={41}&SeasonOrNot={42}&StartDate={43}&EndDate={44}&PackageOrNot={45}&MobileOrNot={46}&HomeOrNot={47}&SecretGoodsYN={48}&LuckyBagGoodsYN={49}&PlayDate={50}&PlaySeq={51}&PlayTime={52}&NoOfTime={53}&OnlineDate={54}&CancelableDate={55}&BookingEndDate={56}&LmtKindOfSettle={57}&LmtCardIssueCode={58}&LmtSalesYN={59}&KindOfDiscount={60}&DiscountName={61}&DiscountAmt={62}&DiscountSeq={63}&TikiKindOfDiscount={64}&OtherKindOfDiscount={65}&CardKindOfDiscount={66}&CardKind={67}&CardDiscountKind={68}&CardIssueCode={69}&CardDiscountNo={70}&CardDelivery={71}&Point={72}&PointSeq={73}&PointDiscountAmt={74}&CouponCode={75}&CouponName={76}&Coupon_DiscountValue={77}&Coupon_TypeOfPay={78}&Coupon_ChargeInfo={79}&Coupon_DblDiscountOrNot={80}&Coupon_DiscountAmt={81}&DeliveryOrNot={82}&DeliveryGift={83}&DeliveryGiftAmt={84}&Delivery={85}&RName={86}&RPhoneNo={87}&RHpNo={88}&RZipcode={89}&RAddr={90}&RSubAddr={91}&MemberName={92}&SSN={93}&PhoneNo={94}&HpNo={95}&Email={96}&SmsOrNot={97}&Zipcode={98}&Addr={99}&SubAddr={100}&DeliveryEnc={101}&MCash_TelKind={102}&MCash_No={103}&MCash_SSN={104}&MCash_Smsval={105}&MCash_Phoneid={106}&MCash_Traid={107}&MCash_UseAmt={108}&MCash_TotalAmt={109}&KindOfSettle={110}&CartID={111}&CartIDSeq={112}&DoubleChecked={113}&PaymentDouble={114}&TotGiftSttledAmt={115}&ISPDiscountOrNot={116}&DiscountOrNot={117}&TicketCntTikiDiscount={118}&FirstKindOfPayment={119}&SecondKindOfPayment={120}&FirstSettleAmt={121}&SecondSettleAmt={122}&BankCode={123}&KindOfCard={124}&DiscountCard={125}&CardNo={126}&ValidInfo={127}&CardSSN={128}&CardPWD={129}&HalbuMonth={130}&LGPoint={131}&WooriPoint={132}&CitiPoint={133}&KBPoint={134}&MPoint={135}&SSPoint={136}&KEBPoint={137}&HashPrice={138}&CultrueUser={139}&HappyUser={140}&FirstSubKindOfPayment={141}&PaymentEnc={142}&PayDiscountCode={143}&Encrypted={144}&RN={145}&IspCardCode={146}&CardCode={147}&CardQuota={148}&BearsNumber={149}&GSPwd={150}&GSCardNo={151}&GSType={152}&GSPointUseYN={153}&GSPointSaveYN={154}&useGSPoint={155}&GSPointCheckYN={156}&OtherSiteGSCardNo={157}&OtherSiteGSYN={158}&OtherSiteGSPwd={159}&expoRecommander={160}&expoRecOrg={161}&expoName={162}&expoImg={163}&FCSeasonNo={164}&PGType={165}&SeatGrade={166}&PriceGrade={167}&SalesPrice={168}&DblDiscountOrNot={169}&GroupId={170}&SeatGradeName={171}&PriceGradeName={172}&BlockNo={173}&Floor={174}&RowNo={175}&SeatNo={176}&VoucherCode={177}&VoucherSalesPrice={178}&DiscountCode={179}&SeatInfo={180}&PGType={181}&SeatGrade={182}&PriceGrade={183}&SalesPrice={184}&DblDiscountOrNot={185}&GroupId={186}&SeatGradeName={187}&PriceGradeName={188}&BlockNo={189}&Floor={190}&RowNo={191}&SeatNo={192}&VoucherCode={193}&VoucherSalesPrice={194}&DiscountCode={195}&SeatInfo={196}",
                       sessionInfo.Tiki.UrlDecode(), sessionInfo.TikiGrade.UrlDecode(), sessionInfo.TikiDiscountCnt.UrlDecode(), sessionInfo.BizCode.UrlDecode(), sessionInfo.GoodsBizCode.UrlDecode(), sessionInfo.GoodsBizName.UrlDecode(), sessionInfo.BizMemberCode.UrlDecode(), sessionInfo.SessionId.UrlDecode(), sessionInfo.BadUserOrNot.UrlDecode(), sessionInfo.TMemHash.UrlDecode(), sessionInfo.SIDBizCode.UrlDecode(), sessionInfo.TicketAmt.UrlDecode(), sessionInfo.TicketCnt.UrlDecode(), sessionInfo.TotalAmt.UrlDecode(), sessionInfo.TotalUseAmt.UrlDecode(), sessionInfo.DelyAmt.UrlDecode(), sessionInfo.VoucherAmt.UrlDecode(), sessionInfo.GroupCode.UrlDecode(), sessionInfo.GoodsCode.UrlDecode(), sessionInfo.PlaceCode.UrlDecode(), sessionInfo.OnlyDeliver.UrlDecode(), sessionInfo.DBDay.UrlDecode(), sessionInfo.ExpressDelyDay.UrlDecode(), sessionInfo.DelyFee.UrlDecode(), sessionInfo.ExpressDelyFee.UrlDecode(), sessionInfo.ReservedOrNot.UrlDecode(), sessionInfo.Always.UrlDecode(), sessionInfo.GroupName.UrlDecode(), sessionInfo.PlaceName.UrlDecode(), sessionInfo.LocOfImage.UrlDecode(), sessionInfo.TmgsOrNot.UrlDecode(), sessionInfo.KindOfGoods.UrlDecode(), sessionInfo.SubCategory.UrlDecode(), sessionInfo.GoodsOption.UrlDecode(), sessionInfo.UseAmt.UrlDecode(), sessionInfo.TypeOfUse.UrlDecode(), sessionInfo.PenaltyScript.UrlDecode(), sessionInfo.SupplyType.UrlDecode(), sessionInfo.RcptTaxYN.UrlDecode(), sessionInfo.HotSaleOrNot.UrlDecode(), sessionInfo.AgreeOrNot.UrlDecode(), sessionInfo.UserInfoOrNot.UrlDecode(), sessionInfo.SeasonOrNot.UrlDecode(), sessionInfo.StartDate.UrlDecode(), sessionInfo.EndDate.UrlDecode(), sessionInfo.PackageOrNot.UrlDecode(), sessionInfo.MobileOrNot.UrlDecode(), sessionInfo.HomeOrNot.UrlDecode(), sessionInfo.SecretGoodsYN.UrlDecode(), sessionInfo.LuckyBagGoodsYN.UrlDecode(), sessionInfo.PlayDate.UrlDecode(), sessionInfo.PlaySeq.UrlDecode(), sessionInfo.PlayTime.UrlDecode(), sessionInfo.NoOfTime.UrlDecode(), sessionInfo.OnlineDate.UrlDecode(), sessionInfo.CancelableDate.UrlDecode(), sessionInfo.BookingEndDate.UrlDecode(), sessionInfo.LmtKindOfSettle.UrlDecode(), sessionInfo.LmtCardIssueCode.UrlDecode(), sessionInfo.LmtSalesYN.UrlDecode(), sessionInfo.KindOfDiscount.UrlDecode(), sessionInfo.DiscountName.UrlDecode(), sessionInfo.DiscountAmt.UrlDecode(), sessionInfo.DiscountSeq.UrlDecode(), sessionInfo.TikiKindOfDiscount.UrlDecode(), sessionInfo.OtherKindOfDiscount.UrlDecode(), sessionInfo.CardKindOfDiscount.UrlDecode(), sessionInfo.CardKind.UrlDecode(), sessionInfo.CardDiscountKind.UrlDecode(), sessionInfo.CardIssueCode.UrlDecode(), sessionInfo.CardDiscountNo.UrlDecode(), sessionInfo.CardDelivery.UrlDecode(), sessionInfo.Point.UrlDecode(), sessionInfo.PointSeq.UrlDecode(), sessionInfo.PointDiscountAmt.UrlDecode(), sessionInfo.CouponCode.UrlDecode(), sessionInfo.CouponName.UrlDecode(), sessionInfo.Coupon_DiscountValue.UrlDecode(), sessionInfo.Coupon_TypeOfPay.UrlDecode(), sessionInfo.Coupon_ChargeInfo.UrlDecode(), sessionInfo.Coupon_DblDiscountOrNot.UrlDecode(), sessionInfo.Coupon_DiscountAmt.UrlDecode(), sessionInfo.DeliveryOrNot.UrlDecode(), sessionInfo.DeliveryGift.UrlDecode(), sessionInfo.DeliveryGiftAmt.UrlDecode(), sessionInfo.Delivery.UrlDecode(), sessionInfo.RName.UrlDecode(), sessionInfo.RPhoneNo.UrlDecode(), sessionInfo.RHpNo.UrlDecode(), sessionInfo.RZipcode.UrlDecode(), sessionInfo.RAddr.UrlDecode(), sessionInfo.RSubAddr.UrlDecode(), sessionInfo.MemberName.UrlDecode(), sessionInfo.SSN.UrlDecode(), sessionInfo.PhoneNo.UrlDecode(), sessionInfo.HpNo.UrlDecode(), sessionInfo.Email.UrlDecode(), sessionInfo.SmsOrNot.UrlDecode(), sessionInfo.Zipcode.UrlDecode(), sessionInfo.Addr.UrlDecode(), sessionInfo.SubAddr.UrlDecode(), sessionInfo.DeliveryEnc.UrlDecode(), sessionInfo.MCash_TelKind.UrlDecode(), sessionInfo.MCash_No.UrlDecode(), sessionInfo.MCash_SSN.UrlDecode(), sessionInfo.MCash_Smsval.UrlDecode(), sessionInfo.MCash_Phoneid.UrlDecode(), sessionInfo.MCash_Traid.UrlDecode(), sessionInfo.MCash_UseAmt.UrlDecode(), sessionInfo.MCash_TotalAmt.UrlDecode(), sessionInfo.KindOfSettle.UrlDecode(), sessionInfo.CartID.UrlDecode(), sessionInfo.CartIDSeq.UrlDecode(), sessionInfo.DoubleChecked.UrlDecode(), sessionInfo.PaymentDouble.UrlDecode(), sessionInfo.TotGiftSttledAmt.UrlDecode(), sessionInfo.ISPDiscountOrNot.UrlDecode(), sessionInfo.DiscountOrNot.UrlDecode(), sessionInfo.TicketCntTikiDiscount.UrlDecode(), sessionInfo.FirstKindOfPayment.UrlDecode(), sessionInfo.SecondKindOfPayment.UrlDecode(), sessionInfo.FirstSettleAmt.UrlDecode(), sessionInfo.SecondSettleAmt.UrlDecode(), sessionInfo.BankCode.UrlDecode(), sessionInfo.KindOfCard.UrlDecode(), sessionInfo.DiscountCard.UrlDecode(), sessionInfo.CardNo.UrlDecode(), sessionInfo.ValidInfo.UrlDecode(), sessionInfo.CardSSN.UrlDecode(), sessionInfo.CardPWD.UrlDecode(), sessionInfo.HalbuMonth.UrlDecode(), sessionInfo.LGPoint.UrlDecode(), sessionInfo.WooriPoint.UrlDecode(), sessionInfo.CitiPoint.UrlDecode(), sessionInfo.KBPoint.UrlDecode(), sessionInfo.MPoint.UrlDecode(), sessionInfo.SSPoint.UrlDecode(), sessionInfo.KEBPoint.UrlDecode(), sessionInfo.HashPrice.UrlDecode(), sessionInfo.CultrueUser.UrlDecode(), sessionInfo.HappyUser.UrlDecode(), sessionInfo.FirstSubKindOfPayment.UrlDecode(), sessionInfo.PaymentEnc.UrlDecode(), sessionInfo.PayDiscountCode.UrlDecode(), sessionInfo.Encrypted.UrlDecode(), sessionInfo.RN.UrlDecode(), sessionInfo.IspCardCode.UrlDecode(), sessionInfo.CardCode.UrlDecode(), sessionInfo.CardQuota.UrlDecode(), sessionInfo.BearsNumber.UrlDecode(), sessionInfo.GSPwd.UrlDecode(), sessionInfo.GSCardNo.UrlDecode(), sessionInfo.GSType.UrlDecode(), sessionInfo.GSPointUseYN.UrlDecode(), sessionInfo.GSPointSaveYN.UrlDecode(), sessionInfo.useGSPoint.UrlDecode(), sessionInfo.GSPointCheckYN.UrlDecode(), sessionInfo.OtherSiteGSCardNo.UrlDecode(), sessionInfo.OtherSiteGSYN.UrlDecode(), sessionInfo.OtherSiteGSPwd.UrlDecode(), sessionInfo.expoRecommander.UrlDecode(), sessionInfo.expoRecOrg.UrlDecode(), sessionInfo.expoName.UrlDecode(), sessionInfo.expoImg.UrlDecode(), sessionInfo.FCSeasonNo.UrlDecode(), sessionInfo.PGType.UrlDecode(), sessionInfo.SeatGrade.UrlDecode(), sessionInfo.PriceGrade.UrlDecode(), sessionInfo.SalesPrice.UrlDecode(), sessionInfo.DblDiscountOrNot.UrlDecode(), sessionInfo.GroupId.UrlDecode(), sessionInfo.SeatGradeName.UrlDecode(), sessionInfo.PriceGradeName.UrlDecode(), sessionInfo.BlockNo.UrlDecode(), sessionInfo.Floor.UrlDecode(), sessionInfo.RowNo.UrlDecode(), sessionInfo.SeatNo.UrlDecode(), sessionInfo.VoucherCode.UrlDecode(), sessionInfo.VoucherSalesPrice.UrlDecode(), sessionInfo.DiscountCode.UrlDecode(), sessionInfo.SeatInfo.UrlDecode(), sessionInfo.PGType.UrlDecode(), sessionInfo.SeatGrade.UrlDecode(), sessionInfo.PriceGrade.UrlDecode(), sessionInfo.SalesPrice.UrlDecode(), sessionInfo.DblDiscountOrNot.UrlDecode(), sessionInfo.GroupId.UrlDecode(), sessionInfo.SeatGradeName.UrlDecode(), sessionInfo.PriceGradeName.UrlDecode(), sessionInfo.BlockNo.UrlDecode(), sessionInfo.Floor.UrlDecode(), sessionInfo.RowNo.UrlDecode(), sessionInfo.SeatNo.UrlDecode(), sessionInfo.VoucherCode.UrlDecode(), sessionInfo.VoucherSalesPrice.UrlDecode(), sessionInfo.DiscountCode.UrlDecode(), sessionInfo.SeatInfo.UrlDecode());

                var httpWRequest =
                    (HttpWebRequest)
                        WebRequest.Create("http://ticket.interpark.com/Book/BookEnd.asp");
                httpWRequest.Accept = "text/html, application/xhtml+xml, */*";
                httpWRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
                httpWRequest.Headers.Add("Accept-Language", "ko-KR");
                httpWRequest.Referer = "http://ticket.interpark.com/Book/BookMain.asp";
                httpWRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
                httpWRequest.Headers.Add("DNT", "1");
                httpWRequest.ContentType = "application/x-www-form-urlencoded;";
                httpWRequest.KeepAlive = true;
                httpWRequest.Method = "Post";
                httpWRequest.ContentLength = parameter.Length;
                httpWRequest.CookieContainer = new CookieContainer();
                httpWRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Util.SetCookie(cookie, httpWRequest);

                var sw = new StreamWriter(httpWRequest.GetRequestStream(), Encoding.GetEncoding("euc-kr"));
                sw.Write(parameter);
                sw.Close();

                var theResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var sr = new StreamReader(theResponse.GetResponseStream(), Encoding.GetEncoding("euc-kr"));

                string resultHtml = sr.ReadToEnd();
                if (resultHtml.Contains("고객님의 결제가 정상적으로 완료되었습니다"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }
    }
}
