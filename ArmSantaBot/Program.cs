using System;
using System.Threading;
using System.Text;
using Tweetinvi;
using System.Configuration;
namespace ArmSantaBot
{
    internal class Program
    {
        private static string customer_key = ConfigurationManager.AppSettings["CustomerKey"];
        private static string customer_key_secret = ConfigurationManager.AppSettings["CustomerKeySecret"];
        private static string access_token = ConfigurationManager.AppSettings["AccessToken"];
        private static string access_token_secret = ConfigurationManager.AppSettings["AccessTokenSecret"];

        private static Random rnd = new Random((int)DateTime.Now.Ticks);

        /*private static string[] keyReply = {
            "ա", "ե", "ի", "ո" ,"ը", "է", "օ", "ու"
        };*/

        private static string[] msgStart = {
            "Ամանորին", "Նոր տարվան", "Մեր սիրելի տոնին", "Նվերներին", "Խոզի բուդին",
            "Բամբանեռկեքին", "Նավասարդին", "Նյու յերին", "Պուտինի ելույթին",
            "Նովիյ' գոդին", "Բլինչիկներին", "Կոկա կոլայի ռեկլամներին", "Էս տարվա ավարտին",
            "Հունվարի 1-ին", "Մյուս տարվա սկզբին", "Սալյուտներին"
        };

        private static string[] msgDescr = {
            "", "կակոյ նիբուդ\' ", "ոմն ", "երկար թվացող ", "ինչ որ ", "մոտ ",
            "անիմաստ թվացող ", "սամֆինգ լայք ", "նեյտրալ ", "մի ամբողջ կյանք՝ ", "ուղիղ ", "սաղ-սաղ ",
            "մաքսիմում ", "քարը տրաքի ", " ", "վրոդի ",
        };

        private static char[,] characters = {
            {'▓', '░'}, {'⣿','⣀'}, {'⬤','○'}, {'■','⬚'}, {'▰', '▱'}, {'◼', '□'}, {'▮', '▯'}, {'⚫', '⚪'} ,
            {'+', '-'}, {'♥','♡'}, {'♚', '♔'}, {'✦', '✧'}, {'昨','将'},
        };

        private static void StartTimeConfigurer()
        {
            var now = DateTime.Now;
            var nextDay = DateTime.Now.AddDays(1);

            nextDay = nextDay.AddHours(nextDay.Hour * -1);
            nextDay = nextDay.AddMinutes(nextDay.Minute * -1);
            nextDay = nextDay.AddSeconds(nextDay.Second * -1);
            //nextDay = nextDay.AddMilliseconds(nextDay.Millisecond * -1);

            Console.WriteLine($"Now = {now}\nextDay = {nextDay}");
            //new DateTime(now.Year, now.Month, now.Day + 1);
            int SecondsLeft = (int)(nextDay - now).TotalSeconds + 5;
            Console.WriteLine("Seconds to wait: {0}", SecondsLeft);
            Thread.Sleep(SecondsLeft * 1000);
        }

        private static string PercentageDrawer(int day, bool leapYear = false)
        {
            int daysInYear = leapYear ? 366 : 365;
            day--;
            //Console.WriteLine("characters.Length / characters.Rank is {0}", characters.Length / characters.Rank);
            int charmode = rnd.Next(0, characters.Length / characters.Rank);
            bool meet6 = false;

            StringBuilder result = new StringBuilder(14);
            int percentage = (day * 100) / daysInYear;
            //Console.WriteLine(percentage);

            for (int iterator = 1; iterator <= 10; iterator++)
            {
                if (iterator == 6 && meet6 == false)
                {
                    result.Append($" {percentage}% ");
                    iterator = 5;
                    meet6 = true;
                    continue;
                }

                if (iterator * 10 <= percentage)
                {
                    result.Append(characters[charmode, 0]);
                    continue;
                }
                result.Append(characters[charmode, 1]);
            }
            return result.ToString();
        }

        public static void Main(string[] args)
        {
            string tweet;
            int daysLeft, startIndex, descrIndex;
            DateTime now;
            DateTime end;
            bool uniqueMessage;

            Auth.SetUserCredentials(customer_key, customer_key_secret, access_token, access_token_secret);

            while (true)
            {
                /*Thread[] threads = null;{new Thread(() => ReplyFunc("Նոր տարի")), new Thread(() => ReplyFunc("Ձմեռ")),
                    new Thread(() => ReplyFunc("Ամանոր")), new Thread(() => ReplyFunc("ձմեռ")), new Thread(() => ReplyFunc("dzmer")),
                    new Thread(() => ReplyFunc("nor tari")), new Thread(() => ReplyFunc("nverner")) };

                foreach (var thread in threads)
                {
                    thread.Start();
                    //Thread.Sleep(13 * 1000); //13 seconds
                }*/

                StartTimeConfigurer();
                uniqueMessage = false;
                now = DateTime.Now;
                end = new DateTime(now.Year + 1, 1, 1);
                daysLeft = (int)(end - now).TotalDays + 1;
                uniqueMessage = (rnd.Next(0, 31) >= 25);

                startIndex = uniqueMessage ? rnd.Next(1, msgStart.Length) : 0;
                descrIndex = uniqueMessage ? rnd.Next(1, msgDescr.Length) : 0;

                tweet = (now.Day == 1 && now.Month == 1) ? "Շնորհավոր Նոր Տարի!" : $"{msgStart[startIndex]} մնաց " +
                    $"{msgDescr[descrIndex]}" + $"{daysLeft} օր:";

                Console.WriteLine("Tweeting the tweet: " + tweet);
                tweet += "\n" + PercentageDrawer(now.DayOfYear, leapYear: now.Year % 4 == 0);
                Console.WriteLine("Posting :{0}", tweet);

                Tweet.PublishTweet(tweet);
            }
        }
    }
}