using System.Collections.Generic;
using System.Windows.Forms;

namespace ElstanLab.UI
{
    public static class ControlRegistry
    {
        static Dictionary<string, Control> controls =
            new Dictionary<string, Control>();

        public static void Register(Control control)
        {
            if (string.IsNullOrWhiteSpace(control.Name))
                return;

            if (!controls.ContainsKey(control.Name))
            {
                controls.Add(control.Name, control);
            }
        }

        public static T Get<T>(string name)
            where T : Control
        {
            if (controls.ContainsKey(name))
            {
                return controls[name] as T;
            }

            return null;
        }

        public static Control Get(string name)
        {
            if (controls.ContainsKey(name))
            {
                return controls[name];
            }

            return null;
        }
    }
}