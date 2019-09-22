
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull.Core.Helpers_Extensions.Helpers
{

    public class pagination
    {
        public int start { get; set; }
        public int Count { get; set; }
    }
    public class sort
    {
        public string predicate { get; set; }
        public bool reverse { get; set; }
    }
    public class DataSourceAngular
    {
        public object data { get; set; }
        public int data_count { get; set; }

        public int page_count { get; set; }
    }
    public class Searchable
    {
        public string colname { get; set; }
        public string colvalue { get; set; }
    }
    public class PagingClass
    {
        public PagingClass()
        {
            id = 0;
            Export = false;
        }
        public pagination pagination { get; set; }
        public sort sort { get; set; }
        public string search { get; set; }
        public string search_operator { get; set; }
        public string filter { get; set; }
        public int id { get; set; }
        public string excludedid { get; set; }
        public string AdditionalParameter { get; set; }
        public bool Export { get; set; }
        public string compareType { get; set; }
    }
    public class ProjectListClass : PagingClass
    {
        public ProjectListClass()
        {

        }

        public int PlanId { get; set; }

        public int StrategicGoalId { get; set; }

        public int ManagementId { get; set; }
    }

}