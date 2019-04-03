using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Autonomous_Downloader
{
    public static class Utility
    {
        /// <summary>
        /// Quick, hacky way to clone
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toBeCloned"></param>
        /// <returns></returns>
        public static T Clone<T>(T toBeCloned)
        {
            if (toBeCloned == null)
            {
                throw new ArgumentNullException(nameof(toBeCloned));
            }
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(toBeCloned));
        }

        public static childItem FindVisualChild<childItem>(this DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        public static T FindChildControlByName<T>(this DependencyObject dependencyObject, string name)
        {
            ContentPresenter myContentPresenter = dependencyObject.FindVisualChild<ContentPresenter>();
            DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
            T control = (T)myDataTemplate.FindName(name, myContentPresenter);
            return control;
        }
    }
}
