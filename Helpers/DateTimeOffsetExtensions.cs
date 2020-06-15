using System;

namespace CourseLibrary.API.Helpers
{
    public static class DateTimeOffsetExtensions
    {
        public static int GetCurrentAge(this DateTimeOffset source)
        {
            var currentDate = DateTime.UtcNow;
            int age = currentDate.Year - source.Year;
            if(currentDate < source.AddYears(age))
            {
                age--;
            }
            return age;
        }
    }
}
