using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Dynamic;
using System.Data.Entity;
using HWASS6.Models;
using System.Web.Http.Cors;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using System.Web.Hosting;
using System.Net.Http.Headers;
using System.Data;

namespace HWASS6.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SchoolController : ApiController
    {
        [System.Web.Mvc.Route("api/School/getReportData")]
        [HttpGet]
        public dynamic getReportData(int courseselection)
        {
            SchoolDBEntities db = new SchoolDBEntities();
            db.Configuration.ProxyCreationEnabled = false;
            List<StudentGrade> grades;

            if (courseselection == 1)
            {
                grades = db.StudentGrades.Include(gg => gg.Person1).Include(gg => gg.Course).Include(gg => gg.Course.Department).Where(gr => db.OnsiteCourses.Any(cc => cc.CourseID == gr.CourseID)).ToList();
            }
            else if (courseselection == 2)
            {
                grades = db.StudentGrades.Include(gg => gg.Person1).Include(gg => gg.Course).Include(gg => gg.Course.Department).Where(gr => db.OnlineCourses.Any(cc => cc.CourseID == gr.CourseID)).ToList();
            }
            else
            {
                grades = db.StudentGrades.Include(gg => gg.Person1).Include(gg => gg.Course).Include(gg => gg.Course.Department).ToList();
            }
            return getExpandoReport(grades);
        }

        private dynamic getExpandoReport(List<StudentGrade> grades)
        {
            dynamic outObject = new ExpandoObject();
            var depList = grades.GroupBy(gg => gg.Course.Department.Name);
            List<dynamic> deps = new List<dynamic>();
            foreach (var group in depList)
            {
                dynamic department = new ExpandoObject();
                department.name = group.Key;
                department.average = group.Average(gg => gg.Grade);
                deps.Add(department);
            }
            outObject.Departments = deps;

            var courseList = grades.GroupBy(gg => gg.Course.Title);
            List<dynamic> courseGroups = new List<dynamic>();
            foreach (var group in courseList)
            {
                dynamic course = new ExpandoObject();
                course.title = group.Key;
                course.average = group.Average(gg => gg.Grade);
                List<dynamic> flexiGrades = new List<dynamic>();
                foreach (var items in group)
                {
                    dynamic gradeObj = new ExpandoObject();
                    gradeObj.student = items.Person1.FirstName + " " + items.Person1.LastName;
                    gradeObj.course = items.Course.Title;
                    gradeObj.grade = items.Grade;
                    flexiGrades.Add(gradeObj);
                }
                course.StudentGrades = flexiGrades;
                courseGroups.Add(course);
            }
            outObject.Courses = courseGroups;
            return outObject;
        }
    }

}