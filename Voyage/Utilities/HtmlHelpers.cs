using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace Voyage.Utilities
{
    public static class HtmlHelpers
    {
        //public static IHtmlContent Table(this IHtmlHelper html, string containerName, IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rowData)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("<div class=\"table-responsive\">");
        //    sb.Append("<table class=\"app-table table\">");

        //    sb.Append("<thead>");

        //        //headers
        //        sb.Append("<tr class=\"app-table-row\" >");
        //            sb.Append("<th class=\"app-table-header\" scope=\"col\"></th>");
        //            foreach (string header in headers)
        //            {
        //                sb.Append("<th class=\"app-table-header\" scope=\"col\">");
        //                sb.Append(header);
        //                sb.Append("</th>");
        //            }
        //        sb.Append("</tr>");

        //    sb.Append("</thead>");
        //    sb.Append("<tbody>");

        //    //rows
        //    foreach(var row in rowData)
        //    {
        //        sb.Append("<tr class=\"app-table-row\">");

        //            //checkbox
        //            sb.Append("<td class=\"app-table-data\">");
        //                sb.Append("<input type=\"checkbox\" />");
        //            sb.Append("</td>");

        //        foreach (var data in row)
        //        {
        //            sb.Append("<td class=\"app-table-data\">");
        //                sb.Append(data);
        //            sb.Append("</td>");
        //        }
                    
        //        sb.Append("</tr>");
        //    }

        //    sb.Append("</tbody>");
        //    sb.Append("</table>");
        //    sb.Append("</div>");

        //    return new HtmlString(sb.ToString());
        //}
        
    }
}

