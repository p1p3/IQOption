namespace IQOptionClient.Ws.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public  class Profile
    {
        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("confirmation_required")]
        public long ConfirmationRequired { get; set; }

        [JsonProperty("popup")]
        public Popup Popup { get; set; }

        [JsonProperty("money")]
        public Money Money { get; set; }

        [JsonProperty("user_group")]
        public string UserGroup { get; set; }

        [JsonProperty("welcome_splash")]
        public long WelcomeSplash { get; set; }

        [JsonProperty("functions")]
        public Functions Functions { get; set; }

        [JsonProperty("finance_state")]
        public string FinanceState { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }

        [JsonProperty("bonus_wager")]
        public double BonusWager { get; set; }

        [JsonProperty("bonus_total_wager")]
        public long BonusTotalWager { get; set; }

        [JsonProperty("balance_id")]
        public long BalanceId { get; set; }

        [JsonProperty("balance_type")]
        public long BalanceType { get; set; }

        [JsonProperty("messages")]
        public long Messages { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("demo")]
        public long Demo { get; set; }

        [JsonProperty("public")]
        public long Public { get; set; }

        [JsonProperty("group_id")]
        public long GroupId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("currency_char")]
        public string CurrencyChar { get; set; }

        [JsonProperty("mask")]
        public string Mask { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("last_visit")]
        public bool LastVisit { get; set; }

        [JsonProperty("site_id")]
        public long SiteId { get; set; }

        [JsonProperty("tz")]
        public string Tz { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("birthdate")]
        public bool Birthdate { get; set; }

        [JsonProperty("country_id")]
        public long CountryId { get; set; }

        [JsonProperty("currency_id")]
        public long CurrencyId { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("postal_index")]
        public string PostalIndex { get; set; }

        [JsonProperty("timediff")]
        public long Timediff { get; set; }

        [JsonProperty("tz_offset")]
        public long TzOffset { get; set; }

        [JsonProperty("balances")]
        public List<Balance> Balances { get; set; }

        [JsonProperty("infeed")]
        public long Infeed { get; set; }

        [JsonProperty("confirmed_phones")]
        public List<string> ConfirmedPhones { get; set; }

        [JsonProperty("need_phone_confirmation")]
        public bool NeedPhoneConfirmation { get; set; }

        [JsonProperty("rate_in_one_click")]
        public bool RateInOneClick { get; set; }

        [JsonProperty("deposit_in_one_click")]
        public bool DepositInOneClick { get; set; }

        [JsonProperty("kyc_confirmed")]
        public bool KycConfirmed { get; set; }

        [JsonProperty("kyc")]
        public Kyc Kyc { get; set; }

        [JsonProperty("trade_restricted")]
        public bool TradeRestricted { get; set; }

        [JsonProperty("auth_two_factor")]
        public object AuthTwoFactor { get; set; }

        [JsonProperty("deposit_count")]
        public long DepositCount { get; set; }

        [JsonProperty("is_activated")]
        public bool IsActivated { get; set; }

        [JsonProperty("new_email")]
        public string NewEmail { get; set; }

        [JsonProperty("tc")]
        public bool Tc { get; set; }

        [JsonProperty("trial")]
        public bool Trial { get; set; }

        [JsonProperty("is_islamic")]
        public bool IsIslamic { get; set; }

        [JsonProperty("tin")]
        public string Tin { get; set; }

        [JsonProperty("flag")]
        public string Flag { get; set; }

        [JsonProperty("cashback_level_info")]
        public CashbackLevelInfo CashbackLevelInfo { get; set; }

        [JsonProperty("user_circle")]
        public string UserCircle { get; set; }

        [JsonProperty("ssid")]
        public string Ssid { get; set; }

        [JsonProperty("skey")]
        public string Skey { get; set; }

        [JsonProperty("connection_info")]
        public List<string> ConnectionInfo { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("modified")]
        public long Modified { get; set; }
    }

    public class Balance
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("new_amount")]
        public long NewAmount { get; set; }

        [JsonProperty("bonus_amount")]
        public long BonusAmount { get; set; }

        [JsonProperty("bonus_total_amount")]
        public long BonusTotalAmount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }
    }

    public  class CashbackLevelInfo
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }

    public class Functions
    {
        [JsonProperty("is_bonus_block")]
        public long IsBonusBlock { get; set; }

        [JsonProperty("is_trading_bonus_block")]
        public long IsTradingBonusBlock { get; set; }

        [JsonProperty("is_vip_mode")]
        public long IsVipMode { get; set; }

        [JsonProperty("is_no_currency_change")]
        public long IsNoCurrencyChange { get; set; }

        [JsonProperty("popup_ids")]
        public List<string> PopupIds { get; set; }
    }

    public class Kyc
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("isRegulatedUser")]
        public bool IsRegulatedUser { get; set; }

        [JsonProperty("isProfileNeeded")]
        public bool IsProfileNeeded { get; set; }

        [JsonProperty("isPhoneNeeded")]
        public bool IsPhoneNeeded { get; set; }

        [JsonProperty("isDocumentsNeeded")]
        public bool IsDocumentsNeeded { get; set; }

        [JsonProperty("isDocumentsDeclined")]
        public bool IsDocumentsDeclined { get; set; }

        [JsonProperty("isProfileFilled")]
        public bool IsProfileFilled { get; set; }

        [JsonProperty("isPhoneFilled")]
        public bool IsPhoneFilled { get; set; }

        [JsonProperty("isDocumentsUploaded")]
        public bool IsDocumentsUploaded { get; set; }

        [JsonProperty("isPhoneConfirmationSkipped")]
        public bool IsPhoneConfirmationSkipped { get; set; }

        [JsonProperty("isPhoneConfirmed")]
        public bool IsPhoneConfirmed { get; set; }

        [JsonProperty("isDocumentsUploadSkipped")]
        public bool IsDocumentsUploadSkipped { get; set; }

        [JsonProperty("isDocumentsApproved")]
        public bool IsDocumentsApproved { get; set; }

        [JsonProperty("isDocumentPoiUploaded")]
        public bool IsDocumentPoiUploaded { get; set; }

        [JsonProperty("isDocumentPoaUploaded")]
        public bool IsDocumentPoaUploaded { get; set; }

        [JsonProperty("daysLeftToVerify")]
        public long DaysLeftToVerify { get; set; }
    }

    public class Money
    {
        [JsonProperty("deposit")]
        public Deposit Deposit { get; set; }

        [JsonProperty("withdraw")]
        public Deposit Withdraw { get; set; }
    }

    public class Deposit
    {
        [JsonProperty("min")]
        public long Min { get; set; }

        [JsonProperty("max")]
        public long Max { get; set; }
    }

    public class Popup
    {
        [JsonProperty("1")]
        public The1 The1 { get; set; }
    }

    public  class The1
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("bonus_code")]
        public string BonusCode { get; set; }

        [JsonProperty("countdown")]
        public long Countdown { get; set; }
    }
}
