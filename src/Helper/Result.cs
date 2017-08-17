using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace W.WebApi.Helper
{
    public class Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int count { get; set; }
        public object data { get; set; }

        public string toString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public Result(bool success = false, string message = null, int count = 0, object data = null)
        {
            if (data == null)
                data = string.Empty;

            this.success = success;
            this.message = message;
            this.count = count;
            this.data = data;
        }

        public static Result SAVE_SUCCESS = new Result() { success = true, message = "保存成功" };
        public static Result SAVE_FAILURE = new Result() { message = "保存失败" };
        public static Result DELETE_SUCCESS = new Result() { success = true, message = "删除成功" };
        public static Result DELETE_FAILURE = new Result() { message = "删除失败" };
        public static Result QUERY_FAILURE = new Result() { message = "查询失败" };

        public static Result QUERY_SUCCESS(object data)
        {
            return new Result() { success = true, message = "查询成功", data = data };
        }

        public static Result QUERY_SUCCESS(object data, int count = 0)
        {
            return new Result() { success = true, message = "查询成功", data = data, count = count };
        }

        public static Result FAILURE(string message)
        {
            return new Result() { message = message };
        }


    }

    public class Result<T>
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int count { get; set; }
        public IList<T> data { get; set; }

        public string toString()
        {
            var iso = new IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            return JsonConvert.SerializeObject(this, iso);
        }

        public Result(bool success = false, string message = null, int count = 0, IList<T> data = null)
        {
            if (data == null)
                data = new List<T>();

            this.success = success;
            this.message = message;
            this.count = count == 0 ? data.Count : count;
            this.data = data;
        }

        public static Result<T> QUERY_SUCCESS(IList<T> data)
        {
            return new Result<T>() { success = true, message = "查询成功", data = data };
        }
        public static Result<T> QUERY_SUCCESS(IList<T> data, int count)
        {
            return new Result<T>() { success = true, message = "查询成功", data = data, count = count };
        }
        public static Result<T> FAILURE(string message)
        {
            return new Result<T>() { message = message };
        }

        public static Result<T> QUERY_FAILURE = new Result<T>() { message = "查询失败" };


    }
}
