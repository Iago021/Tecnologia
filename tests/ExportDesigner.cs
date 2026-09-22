using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web.Script.Serialization;
using System.Windows.Forms;

// Ferramenta temporária de migração: captura o layout existente, sem acessar banco.
internal static class ExportDesigner
{
    static readonly string[] Properties = {
        "Text", "AccessibleName", "Location", "Size", "MinimumSize", "MaximumSize", "Font", "BackColor", "ForeColor", "Dock", "Anchor", "Margin", "Padding", "TabIndex", "TabStop",
        "AutoSize", "AutoSizeMode", "AutoScroll", "AutoScrollMinSize", "BackgroundImageLayout", "BorderStyle", "TextAlign", "FlatStyle", "UseVisualStyleBackColor", "UseCompatibleTextRendering",
        "ReadOnly", "Multiline", "MaxLength", "UseSystemPasswordChar", "ScrollBars", "WordWrap", "AcceptsReturn", "AcceptsTab", "CharacterCasing",
        "DropDownStyle", "IntegralHeight", "DropDownWidth", "MaxDropDownItems", "Checked", "CheckAlign", "CheckState", "ThreeState", "Format", "CustomFormat", "ShowCheckBox",
        "Minimum", "Maximum", "Increment", "DecimalPlaces", "ThousandsSeparator", "Value", "FlowDirection", "WrapContents", "SizeMode", "RowCount", "ColumnCount", "GrowStyle",
        "AllowUserToAddRows", "AllowUserToDeleteRows", "AllowUserToResizeRows", "AutoSizeColumnsMode", "AutoSizeRowsMode", "MultiSelect", "SelectionMode", "RowHeadersVisible", "ColumnHeadersHeight", "ColumnHeadersHeightSizeMode", "EnableHeadersVisualStyles", "GridColor", "BackgroundColor"
    };
    static string Literal(object value)
    {
        if (value == null) return "null";
        Type t = value.GetType();
        if (value is string) return new JavaScriptSerializer().Serialize(value);
        if (value is bool) return (bool)value ? "true" : "false";
        if (t.IsEnum) return "(" + t.FullName + ")" + Convert.ToInt32(value);
        if (value is int) return value.ToString();
        if (value is decimal) return ((decimal)value).ToString(CultureInfo.InvariantCulture) + "M";
        if (value is float) return ((float)value).ToString(CultureInfo.InvariantCulture) + "F";
        if (value is Size) { Size v=(Size)value; return "new System.Drawing.Size("+v.Width+", "+v.Height+")"; }
        if (value is Point) { Point v=(Point)value; return "new System.Drawing.Point("+v.X+", "+v.Y+")"; }
        if (value is Padding) { Padding v=(Padding)value; return "new System.Windows.Forms.Padding("+v.Left+", "+v.Top+", "+v.Right+", "+v.Bottom+")"; }
        if (value is Color) { Color v=(Color)value; return "System.Drawing.Color.FromArgb("+v.A+", "+v.R+", "+v.G+", "+v.B+")"; }
        if (value is Font) { Font v=(Font)value; return "new System.Drawing.Font("+Literal(v.Name)+", "+Literal(v.Size)+", (System.Drawing.FontStyle)"+(int)v.Style+")"; }
        return null;
    }
    static List<Control> All(Control c)
    {
        var result = new List<Control> { c };
        if (c is Form || c is Panel || c is TabControl || c is GroupBox)
            foreach (Control child in c.Controls) result.AddRange(All(child));
        return result;
    }
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
        Assembly app = Assembly.LoadFrom(Path.GetFullPath(args[0]));
        Directory.CreateDirectory(args[1]);
        foreach (Type type in app.GetTypes().Where(t => t.IsSubclassOf(typeof(Form))))
        {
            object[] ctor = type.Name == "RecuperarSenhaForm" || type.Name == "RedefinirSenhaForm" ? new object[]{""} : new object[0];
            using (Form form = (Form)Activator.CreateInstance(type, ctor))
            {
                var events = (EventHandlerList)typeof(Component).GetProperty("Events",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(form,null);
                object key = typeof(Form).GetField("EVENT_LOAD",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
                events.RemoveHandler(key,events[key]);
                Size originalSize = form.ClientSize;
                form.Show(); form.ClientSize = originalSize; Application.DoEvents();
                var all = All(form);
                var names = new Dictionary<Control,string>(); names[form] = "$this";
                foreach (FieldInfo f in type.GetFields(BindingFlags.NonPublic|BindingFlags.Instance))
                {
                    Control control = f.GetValue(form) as Control;
                    if (control != null && all.Contains(control)) names[control]=f.Name;
                }
                int next=0;
                foreach (Control c in all)
                {
                    if (names.ContainsKey(c)) continue;
                    string name = c.Name;
                    if (name == "" || names.Values.Contains(name)) name=c.GetType().Name.ToLowerInvariant()+(++next);
                    names[c]=name;
                }
                using (var resources = new ResXResourceWriter(Path.Combine(args[1],type.Name+".resx")))
                {
                    var controls = new List<object>();
                    foreach (Control c in all)
                    {
                        var props=new Dictionary<string,string>();
                        foreach (string name in Properties)
                        {
                            if (c is Form && (name=="Location" || name=="Size" || name=="AutoSize" || name=="AutoScrollMinSize")) continue;
                            if (c is DateTimePicker && name=="Value") continue;
                            PropertyDescriptor p=TypeDescriptor.GetProperties(c)[name];
                            if (p==null || p.IsReadOnly) continue;
                            string literal=Literal(p.GetValue(c)); if(literal!=null) props[name]=literal;
                        }
                        if(c is Form) props["ClientSize"]=Literal(form.ClientSize);
                        if(c.BackgroundImage!=null) { resources.AddResource(names[c]+".BackgroundImage",c.BackgroundImage); props["BackgroundImage"]="((System.Drawing.Image)(resources.GetObject("+Literal(names[c]+".BackgroundImage")+")))"; }
                        PictureBox picture=c as PictureBox;
                        if(picture!=null && picture.Image!=null) { resources.AddResource(names[c]+".Image",picture.Image); props["Image"]="((System.Drawing.Image)(resources.GetObject("+Literal(names[c]+".Image")+")))"; }
                        Button button=c as Button;
                        if(button!=null) { props["FlatAppearance.BorderSize"]=Literal(button.FlatAppearance.BorderSize); props["FlatAppearance.MouseOverBackColor"]=Literal(button.FlatAppearance.MouseOverBackColor); props["FlatAppearance.MouseDownBackColor"]=Literal(button.FlatAppearance.MouseDownBackColor); }
                        LinkLabel link=c as LinkLabel;
                        if(link!=null) { props["LinkColor"]=Literal(link.LinkColor); props["ActiveLinkColor"]=Literal(link.ActiveLinkColor); props["VisitedLinkColor"]=Literal(link.VisitedLinkColor); }
                        var styles=new Dictionary<string,Dictionary<string,string>>();
                        DataGridView grid=c as DataGridView;
                        if(grid!=null)
                        {
                            props["RowTemplate.Height"]=Literal(grid.RowTemplate.Height);
                            foreach(string styleName in new[]{"DefaultCellStyle","ColumnHeadersDefaultCellStyle","AlternatingRowsDefaultCellStyle"})
                            {
                                var style=(DataGridViewCellStyle)grid.GetType().GetProperty(styleName).GetValue(grid,null);
                                var values=new Dictionary<string,string>();
                                foreach(string p in new[]{"BackColor","ForeColor","SelectionBackColor","SelectionForeColor","Font","Alignment","WrapMode","Format"})
                                { object v=style.GetType().GetProperty(p).GetValue(style,null); if(v!=null && (!(v is Color) || !((Color)v).IsEmpty)) values[p]=Literal(v); }
                                styles[styleName]=values;
                            }
                        }
                        TableLayoutPanel table=c as TableLayoutPanel;
                        TableLayoutPanel parent=c.Parent as TableLayoutPanel;
                        var combo=c as ComboBox;
                        controls.Add(new { name=names[c], type=c.GetType().FullName, parent=c==form?null:names[c.Parent], properties=props,
                            rounded=c.Region!=null, tag=c.Tag as string, styles=styles,
                            column=parent==null?-1:parent.GetColumn(c), row=parent==null?-1:parent.GetRow(c),
                            columns=table==null?null:table.ColumnStyles.Cast<ColumnStyle>().Select(s=>new{type=(int)s.SizeType,size=s.Width}).ToArray(),
                            rows=table==null?null:table.RowStyles.Cast<RowStyle>().Select(s=>new{type=(int)s.SizeType,size=s.Height}).ToArray(),
                            items=combo==null?null:combo.Items.Cast<object>().Select(x=>x.ToString()).ToArray() });
                    }
                    File.WriteAllText(Path.Combine(args[1],type.Name+".json"),new JavaScriptSerializer{MaxJsonLength=int.MaxValue}.Serialize(controls));
                }
                Console.WriteLine("EXPORTED "+type.Name);
            }
        }
    }
}
