using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using MTAoarsGeneral.Utilities.Extensions;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Utilities.Mvc
{
    public static class HtmlHelperControlsExtensions
    {

        public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string partialViewName, bool useFieldPrefix = true)
        {
            var name = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            var text = ExpressionHelper.GetExpressionText(expression);
            name = name.GetModelProperty(text);
            var model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;
            var viewData = new ViewDataDictionary(helper.ViewData)
            {
                TemplateInfo = new TemplateInfo
                {
                    HtmlFieldPrefix = useFieldPrefix ? name : string.Empty
                }
            };
            return helper.Partial(partialViewName, model, viewData);

        }

        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<LookupItem> values)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var sb = new StringBuilder();
            foreach (LookupItem item in values)
            {

                var id = string.Format("{0}_{1}", metaData.PropertyName, item.ID);
                var label = item.Description;
                var dictionary = new Dictionary<string, object>();
                dictionary.Add("data-bind", "checked:" + helper.GetName(expression));
                dictionary.Add("id", id);
                var radio = helper.RadioButtonFor(expression, item.ID, dictionary).ToHtmlString();
                sb.AppendFormat("{0}{1}", radio, label);
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<LookupItem> values, object htmlAttributes = null, bool includeDefault = true)
        {
            var list = GetSelectListItem(values, includeDefault);
            return helper.DropDownListFor(expression, list, htmlAttributes);
        }

        const string kendoDropDown = @"<input id='{0}' data-bind='value:{1}' name='{1}' data-value-field='ID' data-text-field='Description' {3}/>
                                        <span class='k-invalid-msg' data-for='{1}'></span>
                                        <script type='text/javascript'>
                                            $(function() {{
                                                $('#{0}').kendoDropDownList({{
                                                    dataValueField:'ID', dataTextField:'Description',
                                                    optionLabel: {{
                                                        ID:null, Description:''
                                                    }},
                                                    dataSource: {{
                                                        type:'jsonp',
                                                        transport:{{read:'{2}'}}
                                                    }}
                                                }});
                                            }});

                                        </script>
                                    ";

        const string template = @"<select id='{0}' multiple='multiple' data-bind='value:{1}' name='{1}' data-value-field='ID' data-text-field='Description' {3}/>
                        <script type='text/javascript'>
                            $(function() {{
                                $('#{0}').kendoMultiSelect({{
                                  
                                    dataValueField: 'ID',
                                    dataTextField: 'Description',
                                    serverFiltering: true, 
                                    width: 200,
                                    className: 'custom-multi-select',
                                    dataSource: {{
                                        type: 'jsonp',
                                        transport: {{
                                            read: '{2}'
                                        }}
                                    }},
                                  dataBound: function(e) {{
                                        $('#{0}').data('kendoMultiSelect').element.find('.k-loading-mask').hide();
                                    }}
                                }});                               
                            }});

                                      
                        </script>";

        const string kendoRelateDropDown = @"<input id='{0}' data-bind='value:{1}' name='{1}' data-value-field='ID' data-text-field='Description' {3}/>
                                        <span class='k-invalid-msg' data-for='{1}'></span>
                                        <script type='text/javascript'>
                                            $(function() {{
                                                $('#{0}').kendoDropDownList({{
                                                    dataValueField:'ID', dataTextField:'Description',
                                                    optionLabel: {{
                                                        ID:null, Description:''
                                                    }},
                                                    dataSource: {{
                                                        type:'jsonp',
                                                        transport:{{read:'{2}'}}
                                                    }},
                                                    change: function(e) {{
                                                        var dropdown = $('#{4}').data('kendoDropDownList');
                                                        dropdown.value(null);
                                                        dropdown.setDataSource(new kendo.data.DataSource({{
                                                            type:'jsonp',
                                                            transport:{{read:'{5}&{0}='+this.value()}}
                                                        }}));
                                                        
                                                    }}
                                                }});
                                            }});
                                        </script>
                                    ";

        public static MvcHtmlString BindDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string readUrl, bool includeDefault = true, bool isReadonly = false, bool required = false)
        {
            /* var list = GetSelectListItem(values, includeDefault);
             var dictionary = helper.GetBindDictionary(expression);
             dictionary.Add("data-role", "dropdownlist");
             dictionary.Add("data-value-field", "ID");
             dictionary.Add("data-text-field", "Description");
             return helper.DropDownListFor(expression, list, dictionary);*/
            var model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model as LookupItem;
            var value = string.Empty;
            if (model != null)
            {
                value = model.Description;
            }
            if (isReadonly)
            {
                return helper.LabelFor(expression, value);
            }
            var dictionary = helper.GetBindDictionary(expression);
            var name = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            var text = ExpressionHelper.GetExpressionText(expression);
            name = name.GetModelProperty(text);
            var id = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(text);
            string requiredAttr = required ? "required" : string.Empty;
            var dropDown = String.Format(kendoDropDown, id, name, readUrl, requiredAttr);
            return MvcHtmlString.Create(dropDown);

        }

        public static MvcHtmlString BindMultiDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string readUrl, bool includeDefault = true, bool isReadonly = false, bool required = false)
        {

            var model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model as LookupItem;
            var value = string.Empty;
            if (model != null)
            {
                value = model.Description;
            }
            if (isReadonly)
            {
                return helper.LabelFor(expression, value);
            }
            var dictionary = helper.GetBindDictionary(expression);
            var name = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            var text = ExpressionHelper.GetExpressionText(expression);
            name = name.GetModelProperty(text);
            var id = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(text);
            string requiredAttr = required ? "required" : string.Empty;
            var dropDown = String.Format(template, id, name, readUrl, requiredAttr);
            return MvcHtmlString.Create(dropDown);

        }
        public static MvcHtmlString BindRelateDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expressionSource, string SourceReadUrl, Expression<Func<TModel, TProperty>> expressionTarget, string TargetReadUrl, bool includeDefault = true, bool isReadonly = false, bool required = false)
        {
            var model = ModelMetadata.FromLambdaExpression(expressionSource, helper.ViewData).Model as LookupItem;
            var value = string.Empty;
            if (model != null)
            {
                value = model.Description;
            }
            if (isReadonly)
            {
                return helper.LabelFor(expressionSource, value);
            }
            var dictionary = helper.GetBindDictionary(expressionSource);
            var name = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            var text = ExpressionHelper.GetExpressionText(expressionSource);
            name = name.GetModelProperty(text);
            var id = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(text);
            string requiredAttr = required ? "required" : string.Empty;
            var textTarget = ExpressionHelper.GetExpressionText(expressionTarget);
            var idTarget = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(textTarget);
            var dropDown = String.Format(kendoRelateDropDown, id, name, SourceReadUrl, requiredAttr, idTarget, TargetReadUrl);
            return MvcHtmlString.Create(dropDown);

        }

        public static MvcHtmlString ListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<LookupItem> values, object htmlAttributes = null, bool includeDefault = true)
        {
            var list = new List<SelectListItem>();
            foreach (LookupItem item in values)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Description,
                    Value = item.ID.ToString()
                });
            }
            return helper.ListBoxFor(expression, list, htmlAttributes);
        }

        public static MvcHtmlString BindListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IEnumerable<LookupItem> values, bool includeDefault = true)
        {
            var list = new List<SelectListItem>();
            foreach (LookupItem item in values)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Description,
                    Value = item.ID.ToString()
                });
            }
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("data-bind", "value:" + helper.GetName(expression));
            dictionary.Add("class", "k-dropdown-list");
            return helper.ListBoxFor(expression, list, dictionary);
        }

        public const string ValMsgForControlText = @"<label class=""error"" style=""display:none""><a href='#'></a><span class=""ui-icon ui-icon-alert"" style=""float: left; margin-right: .3em;""></span>
                <span data-valmsg-for=""{0}""></span></label>";

        public const string ValSummaryForControlText = @"<label class=""error"" ><span data-valsummary=""""><ul class='k-widget k-tooltip k-tooltip-validation mta-error-summary'></ul></span></label>";

        public static MvcHtmlString ValMsgFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var prefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            var text = ExpressionHelper.GetExpressionText(expression);
            var property = prefix.GetModelProperty(text);
            return MvcHtmlString.Create(String.Format(ValMsgForControlText, property));
        }

        public static MvcHtmlString ValSummary(this HtmlHelper helper)
        {
            return MvcHtmlString.Create(ValSummaryForControlText);
        }

        const string ControlLabel = @"<li>
                    <label class=""desc"" for=""Name"">
                        {0}</label>
                    <div>
                       {1}
                       {2}
                    </div>
                </li>";



        public static MvcHtmlString ListItemTextBox<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, bool isMandatory = false, bool isReadonly = false)
        {
            if (isReadonly)
            {
                return helper.LabelFor<TModel, TProperty>(expression, (ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model ?? string.Empty).ToString());
            }

            var textBox = helper.TextBoxFor(expression, new { @class = "k-textbox large" }).ToHtmlString();
            return helper.ListItemControl(expression, textBox, isMandatory);
        }

        public static MvcHtmlString ListItemDropDown<TModel, TProperty>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, TProperty>> expression, IEnumerable<LookupItem> list, bool isMandatory = false, bool isReadonly = false)
        {
            if (isReadonly)
            {
                string value = string.Empty;
                var defaultValue = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model ?? 0;
                var defaultItem = list.SingleOrDefault(i => i.ID == Convert.ToInt64(defaultValue));
                if (defaultItem != null)
                {
                    value = defaultItem.Description;
                }

                return helper.LabelFor<TModel, TProperty>(expression, value);
            }
            var dropDown = helper.DropDownListFor(expression, list, new { style = "width:75%" }).ToHtmlString();
            return helper.ListItemControl(expression, dropDown);
        }

        public static MvcHtmlString ListItemDateEditor<TModel, TProperty>(this HtmlHelper<TModel> helper,
          Expression<Func<TModel, TProperty>> expression, bool isMandatory = false, bool isReadonly = false)
        {
            if (isReadonly)
            {
                return helper.LabelFor<TModel, TProperty>(expression, (ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model ?? string.Empty).ToString());
            }
            var dateEditor = helper.EditorFor(expression, "DateEditor").ToHtmlString();
            return helper.ListItemControl(expression, dateEditor);
        }

        public static MvcHtmlString ListItemControl<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, string control, bool isMandatory = false)
        {
            var label = new StringBuilder(helper.LabelFor(expression).ToHtmlString());
            var valMsg = helper.ValMsgFor(expression);
            if (isMandatory || control.Contains("data-val-required"))
            {
                label.Append("<span class='mandatory'>&nbsp;*</span>");
            }
            var output = String.Format(ControlLabel, label, control, valMsg);
            return MvcHtmlString.Create(output);
        }

        public static MvcHtmlString BindTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression, int maxLength = 0, bool isReadonly = false, bool offautocomplete = false)
        {
            if (isReadonly)
            {
                return helper.LabelFor<TModel, TProperty>(expression, (ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model ?? string.Empty).ToString());
            }
            var dictionary = helper.GetBindDictionary(expression);
            dictionary.Add("class", "k-textbox");
            if (maxLength > 0) dictionary.Add("maxlength", maxLength);
            if (offautocomplete) dictionary.Add("autocomplete", "off");
            return helper.TextBoxFor(expression, dictionary);
        }

        public static MvcHtmlString BindPasswordFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var dictionary = helper.GetBindDictionary(expression);
            dictionary.Add("class", "k-textbox");
            return helper.PasswordFor(expression, dictionary);
        }

        public static MvcHtmlString BindEditorFor<TModel>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, bool>> expression, bool isReadonly = false)
        {

            var dictionary = new Dictionary<string, object>();
            dictionary.Add("data-bind", "checked:" + helper.GetName(expression));
            if (isReadonly)
            {
                dictionary.Add("disabled", "true");
            }
            return helper.CheckBoxFor(expression, dictionary);
        }

        public static MvcHtmlString BindRadionButtonFor<TModel>(this HtmlHelper<TModel> helper,
           Expression<Func<TModel, bool>> expression, object value)
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("data-bind", "checked:" + helper.GetName(expression));
            return helper.RadioButtonFor(expression, value, dictionary);
        }

        public static MvcHtmlString BindDate<Model>(this HtmlHelper<Model> helper,
          string name, object value)
        {
            var dictionary = new Dictionary<string, object>();

            dictionary.Add("data-bind", "value:" + helper.ViewData.TemplateInfo.HtmlFieldPrefix);
            dictionary.Add("data-role", "datepicker");
            dictionary.Add("data-format", GlobalConstants.DateFormat);
            dictionary.Add("pattern", "(0?[1-9]|[12][0-9]|3[01])/(0?[1-9]|1[012])/((19|20)\\d{2})");
            return helper.TextBox(name, value, dictionary);
        }

        static List<SelectListItem> GetSelectListItem(IEnumerable<LookupItem> values, bool includeDefault)
        {
            var list = new List<SelectListItem>();
            if (includeDefault) list.Add(new SelectListItem { Value = "", Text = "Select" });
            foreach (LookupItem item in values)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.Description,
                    Value = item.ID.ToString()
                });
            }
            return list;
        }

        static Dictionary<string, object> GetBindDictionary<TModel, TProperty>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("data-bind", "value:" + helper.GetName(expression));
            return dictionary;
        }

        static string GetName<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var name = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            var text = ExpressionHelper.GetExpressionText(expression);
            return name.GetModelProperty(text);
        }
        public static MvcHtmlString EditorFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string templateName, bool isReadonly)
        {
            if (isReadonly)
            {
                return helper.DisplayFor<TModel, TProperty>(expression);


            }
            return helper.EditorFor<TModel, TProperty>(expression, templateName);

        }

        public static MvcHtmlString EditorFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, bool isReadonly)
        {
            if (isReadonly)
            {

                return helper.DisplayFor<TModel, TProperty>(expression);

            }
            return helper.EditorFor<TModel, TProperty>(expression);

        }



    }// class
}// namespace

