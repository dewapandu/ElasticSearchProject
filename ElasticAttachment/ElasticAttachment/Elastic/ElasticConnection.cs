using ElasticAttachment.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElasticAttachment.Elastic
{
    public class ElasticConnection
    {
        public ElasticClient client()
        {
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(node).DefaultIndex("attachdoc");
            var client = new ElasticClient(settings);
            return client;
        }
         
        public void Index()
        {
            var indexResponse = client().CreateIndex("attachdoc", c => c
              .Settings(s => s
                .Analysis(a => a
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
                    .Object<Attachment>(a => a
                      .Name(n => n.Attachment)
                      .AutoMap()
                    )
                  )
                )
              )
            );
            
            client().PutPipeline("attachments", p => p
              .Description("Document attachment pipeline")
              .Processors(pr => pr
                .Attachment<AttachModel>(a => a
                  .Field(f => f.Content)
                  .TargetField(f => f.Attachment)
                )
                .Remove<AttachModel>(r => r
                  .Field(f => f.Content)
                )
              )
            );
        }
    }
}