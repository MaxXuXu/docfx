// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using Markdig;
using Markdig.Extensions.Yaml;
using Newtonsoft.Json.Linq;

namespace Microsoft.Docs.Build
{
    internal static class ExtractYamlHeader
    {
        public static MarkdownPipelineBuilder UseExtractYamlHeader(
            this MarkdownPipelineBuilder builder,
            Context context,
            Document file,
            StrongBox<JObject> result)
        {
            return builder.Use(document =>
            {
                document.Visit(node =>
                {
                    if (node is YamlFrontMatterBlock yamlHeader)
                    {
                        try
                        {
                            var yamlHeaderObj = YamlUtility.Deserialize(yamlHeader.Lines.ToString());

                            if (yamlHeaderObj is JObject obj)
                            {
                                result.Value = obj;
                            }
                            else
                            {
                                context.ReportWarning(Errors.YamlHeaderNotObject(file, isArray: yamlHeaderObj is JArray));
                            }
                        }
                        catch (Exception ex)
                        {
                            context.ReportWarning(Errors.InvalidYamlHeader(file, ex));
                        }
                        return true;
                    }
                    return false;
                });
            });
        }
    }
}