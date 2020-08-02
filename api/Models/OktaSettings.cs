namespace api.Models
{
    public class OktaSettings
    {
        public string Authority { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }

        public override string ToString()
        {
            return $@"
Okta settings:
    {nameof(Authority)} = '{Authority}'
    {nameof(Issuer)} = '{Issuer}'
    {nameof(Audience)} = '{Audience}'
";
        }
    }

}