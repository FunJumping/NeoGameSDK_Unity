using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

static public class global
{
    public const string cp_adress = "ASZzR4Qm7iVbdUnLrRA7vBMEoX1dnmXeQe";   //公司充值账户
    public const string api = "https://api.nel.group/api/testnet";
    public const string id_GAS = "0x602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7";
    //static public string id_sgas = "0x3f7420285874867c30f32e44f304fd62ad1e9573";
    static public string id_sgas = "0x2761020e5e6dfcd8d37fdd50ff98fa0f93bccf54";
    static public int game_id = 7;
    static public int netType = 2;  //2为测试网

    public static string unicodeToStr(string str)
    {

        string outStr = "";
        string a = "";

        if (!string.IsNullOrEmpty(str))
        {
            for (int i = 0; i < str.Length; i++)
            {
                a = str[i].ToString();
                if (Regex.IsMatch(a, "u") && i + 5 <= str.Length)
                {
                    a = str.Substring(i + 1, 4);
                    outStr += (char)int.Parse(a, System.Globalization.NumberStyles.HexNumber);
                    i += 4;
                }
                else
                {
                    outStr += str[i];

                }
            }
        }
        return outStr;
    }
}
