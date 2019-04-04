using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

using Newtonsoft.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using Instagraph.Data;
using Instagraph.Models;

namespace Instagraph.DataProcessor
{
    public class Deserializer
    {
        public static string ImportPictures(InstagraphContext context, string jsonString)
        {
			throw new NotImplementedException();
        }

        public static string ImportUsers(InstagraphContext context, string jsonString)
        {
            throw new NotImplementedException();
        }

        public static string ImportFollowers(InstagraphContext context, string jsonString)
        {
            throw new NotImplementedException();
        }

        public static string ImportPosts(InstagraphContext context, string xmlString)
        {
            throw new NotImplementedException();
        }

        public static string ImportComments(InstagraphContext context, string xmlString)
        {
            throw new NotImplementedException();
        }
    }
}
