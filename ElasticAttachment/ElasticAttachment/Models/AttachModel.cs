using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElasticAttachment.Models
{
    public class AttachModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
        public Attachment Attachment { get; set; }
    }
}