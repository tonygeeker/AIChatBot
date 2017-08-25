using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Educor.Api.Data.DbContexts;
using Educor.Api.Data.Enums;
using Educor.Api.Data.Models;

namespace Educor.Api.AIBot.Services
{
    public class CourseSearchService
    {
        private readonly List<Course> _courses;

        public IEnumerable<Data.Models.Course> SearchByCategory(string category, int tenantId)
        {
            //var categories = new List<CourseCategory>();
            var repo = new EducorDbRepo(null);
            var courses = repo.CourseQueries.GetAllAvailableForEnrolment(tenantId).Where(x => x.IsVisible == true && x.Name.Contains(category)).ToList();
            return courses;
        }

        public IEnumerable<Data.Models.Course> SearchByName(string courseName, int tenantId)
        {
            var repo = new EducorDbRepo(null);
            var courses = repo.CourseQueries.GetAllForTenant(tenantId).Where(x => x.IsVisible == true && x.Name.Contains(courseName)).ToList();
            return courses;
        }

        public IEnumerable<Data.Models.Course> ShowAll(int tenantId)
        {
            var repo = new EducorDbRepo(null);
            var courses = repo.CourseQueries.GetAllForTenant(tenantId);
            return courses;
        }

        public IEnumerable<Data.Models.Course> SearchByStartDate(string startDate, int tenantId, string inBeforeOrAfter)
        {
            var categories = new List<CourseCategory>();
            var repo = new EducorDbRepo(null);
            var startDateString = startDate.Substring(0, 6);

            IEnumerable<Data.Models.Course> courses = repo.CourseQueries.GetAllAvailableForEnrolment(tenantId)
                        .Where(x => x.Semester.Substring(0, 6).Equals(startDateString)).ToList();
            switch (inBeforeOrAfter)
            {
                case "in":
                    courses = repo.CourseQueries.GetAllAvailableForEnrolment(tenantId)
                        .Where(x =>  x.Semester.Substring(0, 6).Equals(startDateString)).ToList();
                    break;

                case "before":
                    courses = repo.CourseQueries.GetAllAvailableForEnrolment(tenantId)
                        .Where(x => string.Compare(x.Semester.Substring(0, 6), startDateString, false) < 0 ).ToList();
                    break;

                default:
                case "after":
                    courses = repo.CourseQueries.GetAllAvailableForEnrolment(tenantId)
                    .Where(x => string.Compare(x.Semester.Substring(0, 6), startDateString, false) >= 0);
                    break;
            }

            return courses;
        }


    }
}