using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Educor.Api.AIBot
{
    public class Course
    {
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public decimal Price { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime NextStarDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}