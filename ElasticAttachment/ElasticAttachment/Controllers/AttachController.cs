using ElasticAttachment.Elastic;
using ElasticAttachment.Models;
using Nest;
using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ElasticAttachment.Controllers
{
    public class AttachController : Controller
    {
        ElasticConnection elastic = new ElasticConnection();
        // GET: Attach
        public void Index()
        {
            var indexResponse = elastic.client().CreateIndex("attachdoc", c => c
              .Settings(s => s
                .Analysis(an => an
                  .Analyzers(ad => ad
                    .Custom("windows_path_hierarchy_analyzer", ca => ca
                      .Tokenizer("windows_path_hierarchy_tokenizer")
                    )
                  )
                  .Tokenizers(t => t
                    .PathHierarchy("windows_path_hierarchy_tokenizer", ph => ph
                      .Delimiter('\\')
                    )
                  )
                )
              )
              .Mappings(m => m
                .Map<AttachModel>(mp => mp
                  .AutoMap()
                  .AllField(all => all
                    .Enabled(false)
                  )
                  .Properties(ps => ps
                    .Text(s => s
                      .Name(n => n.Path)
                      .Analyzer("windows_path_hierarchy_analyzer")
                    )
                    .Object<Attachment>(at => at
                      .Name(n => n.Attachment)
                      .AutoMap()
                    )
                  )
                )
              )
            );

            elastic.client().PutPipeline("attachments", p => p
              .Description("Document attachment pipeline")
              .Processors(pr => pr
                .Attachment<AttachModel>(am => am
                  .Field(f => f.Content)
                  .TargetField(f => f.Attachment)
                )
                .Remove<AttachModel>(r => r
                  .Field(f => f.Content)
                )
              )
            );

            var directory = Directory.GetCurrentDirectory();
            var base64File = Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(directory,"test.docx")));
            elastic.client().Index(new AttachModel
            {
                Id = 1,
                Path = @"\\Documents\test.docx",
                Content = base64File
            }, i => i.Pipeline("attachments"));
            //return base64File;
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public string Search(string search)
        {
            var searchResponse = elastic.client().Search<AttachModel>(s => s
              .Query(q => q
                .Match(m => m
                  .Field(a => a.Attachment.Content)
                  .Query(search)
                )
              )
            );
            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(searchResponse.Documents);
            return json;
        }
    }
}