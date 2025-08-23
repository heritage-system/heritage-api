using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace Cultural_Heritage_System.Helpers
{


    public static class StringHelper
    {

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
           
            var result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            return result.ToLower();
        }

      
        public static string ToSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            
            var slug = RemoveDiacritics(text);

            
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

          
            slug = Regex.Replace(slug, @"\s+", " ").Trim();

          
            slug = slug.Replace(" ", "-");

            return slug;
        }
    }
}

