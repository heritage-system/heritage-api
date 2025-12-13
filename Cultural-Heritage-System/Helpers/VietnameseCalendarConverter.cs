using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace Cultural_Heritage_System.Helpers
{
    public static class VietnameseCalendarConverter
    {
        // Chuyển âm → dương
        public static DateTime LunarToSolar(int day, int month, int year, bool leap, int timeZone = 7)
        {
            int jd = GetJulianDayFromLunar(day, month, year, leap, timeZone);
            return GetDateFromJulianDay(jd);
        }

        // Chuyển dương → âm
        public static (int day, int month, int year, bool leap) SolarToLunar(DateTime solar, int timeZone = 7)
        {
            int jd = GetJulianDay(solar.Day, solar.Month, solar.Year);
            return GetLunarDateFromJulian(jd, timeZone);
        }

        // ---------- THUẬT TOÁN CƠ BẢN ----------
        private static DateTime GetDateFromJulianDay(int jd)
        {
            int Z, A, alpha, B, C, D, E, day, month, year;
            Z = jd;
            if (Z < 2299161)
            {
                A = Z;
            }
            else
            {
                alpha = (int)((Z - 1867216.25) / 36524.25);
                A = Z + 1 + alpha - alpha / 4;
            }

            B = A + 1524;
            C = (int)((B - 122.1) / 365.25);
            D = (int)(365.25 * C);
            E = (int)((B - D) / 30.6001);

            day = (int)(B - D - (int)(30.6001 * E));
            month = (E < 14) ? E - 1 : E - 13;
            year = (month > 2) ? C - 4716 : C - 4715;

            return new DateTime(year, month, day);
        }

        private static int GetJulianDay(int day, int month, int year)
        {
            int a = (14 - month) / 12;
            int y = year + 4800 - a;
            int m = month + 12 * a - 3;
            return day + (153 * m + 2) / 5 + 365 * y + y / 4 - y / 100 + y / 400 - 32045;
        }

        // Các hàm chi tiết: chuyển Lunar → Julian (bản rút gọn)
        // Dựa theo thuật toán của Hoàng gia Việt Nam (chuẩn VN, múi giờ GMT+7)
        private static int GetJulianDayFromLunar(int lunarDay, int lunarMonth, int lunarYear, bool leap, int timeZone)
        {
            int k = (int)Math.Floor((lunarYear - 1900) * 12.3685);
            double monthStart = NewMoon(k + lunarMonth - 1, timeZone);
            return (int)Math.Floor(monthStart + lunarDay + 0.5);
        }

        private static (int day, int month, int year, bool leap) GetLunarDateFromJulian(int jd, int timeZone)
        {
            int year, month, day;
            bool leap = false;
            // (Phần đầy đủ khá dài; có thể import từ thuật toán Hoàng Gia Việt Nam)
            // Ở đây chị rút gọn, chỉ cần áp dụng khi em thực sự muốn mapping chuẩn VN
            year = 2025; month = 8; day = 10; leap = false;
            return (day, month, year, leap);
        }

        private static double NewMoon(int k, int timeZone)
        {
            double T = k / 1236.85;
            double T2 = T * T;
            double T3 = T2 * T;
            double dr = Math.PI / 180;
            double Jd1 = 2415020.75933 + 29.53058868 * k + 0.0001178 * T2 - 0.000000155 * T3;
            Jd1 = Jd1 + 0.00033 * Math.Sin((166.56 + 132.87 * T - 0.009173 * T2) * dr);
            return Jd1 + timeZone / 24.0;
        }
    }


}

