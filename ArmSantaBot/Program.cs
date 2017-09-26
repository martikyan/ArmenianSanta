﻿using System;
using System.Threading;
using TweetSharp;
using System.Text;

namespace ArmSantaBot
{
	internal class Program
	{
		private static string customer_key = "EbRpdbdaveroRM3vCJBwDyGqC";
		private static string customer_key_secret = "KM05O4q90YspeSaJJ5zt4nR2uppUpztqkaKY7nUnZ3o4psJARQ";
		private static string access_token = "911687372529643522-v5MaXvflkLU9zSKnJaNC3P7Bek0SALw";
		private static string access_token_secret = "zxsmcBgMXodq9pPYNBESwWsSFv5yDLVVHkutp67Q33L3c";

		private static Random rnd = new Random((int)DateTime.Now.Ticks);
		private static TwitterService TService =
			new TwitterService(customer_key, customer_key_secret, access_token, access_token_secret);

		private static string[] msgStart = {
			"Ամանորին", "Նոր տարվան", "Մեր սիրելի տոնին", "Նվերներին", "Խոզի բուդին",
			"Բամբանեռկեքին", "Նավասարդին", "Նյու յերին", "Պուտինի ելույթին",
			"Նովիյ' գոդին"
		};

		private static string[] msgDescr = {
			"", "կակոյ նիբուդ\' ", "ոմն ", "երկար թվացող ", "ինչ որ ", "մոտ ",
			"անիմաստ թվացող ", "սամֆինգ լայք ", "նեյտրալ ", "մի ամբողջ կյանք՝ ", "ուղիղ ", "սաղ-սաղ ",
		};

        private static char[,] characters = {
            {'▓', '░'}, {'⣿','⣀'}, {'⬤','○'}, {'■','□'}, {'▰', '▱'}, {'◼', '▭'}, {'▮', '▯'}, {'⚫', '⚪'} , {'+', '-'}
        };

		private static void StartTimeConfigurer()
		{
            var now = DateTime.Now;
            var nextDay = new DateTime(now.Year, now.Month, now.Day + 1);
            int SecondsLeft = (int)(nextDay - now).TotalSeconds + 5;
			Console.WriteLine("Seconds to wait: {0}", SecondsLeft);
			Thread.Sleep(SecondsLeft);
		}

        private static string PercentageDrawer(int day, bool leapYear = false){
            int daysInYear = leapYear ? 366 : 365;
            day--;
            //Console.WriteLine("characters.Length / characters.Rank is {0}", characters.Length / characters.Rank);
            int charmode = rnd.Next(0, characters.Length / characters.Rank);
            bool meet6 = false;

            StringBuilder result = new StringBuilder(14);
            int percentage = day * 100 / daysInYear;
            //Console.WriteLine(percentage);

            for (int iterator = 1; iterator <= 10; iterator++)
            {
                if (iterator == 6 && meet6 == false) {
                    result.Append($" {percentage}% ");
                    iterator = 4;
                    meet6 = true;
                    continue;
                }

                if(iterator * 10 <= percentage)
                {
                    result.Append(characters[charmode,0]);
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

			while (true)
			{
				StartTimeConfigurer();
                uniqueMessage = false;
				now = DateTime.Now;
				end = new DateTime(now.Year + 1, 1, 1);
				daysLeft = (int)(end - now).TotalDays;
                uniqueMessage = (rnd.Next(0, 31) >= 25);

                startIndex = uniqueMessage ? rnd.Next(1, msgStart.Length) : 0;
                descrIndex = uniqueMessage ? rnd.Next(1, msgDescr.Length) : 0;

                tweet = (now.Day == 1) ? "Շնորհավոր Նոր Տարի!" : $"{msgStart[startIndex]} մնաց " +
                    $"{msgDescr[descrIndex]}" + $"{daysLeft} օր:";

				//Console.WriteLine("Tweeting the tweet: " + tweet);
                tweet += "\n" + PercentageDrawer(now.DayOfYear, leapYear: now.Year % 4 == 0);
                Console.WriteLine("Posting :{0}", tweet);
				TService.SendTweet(new SendTweetOptions { Status = tweet });
			}
		}
	}
}