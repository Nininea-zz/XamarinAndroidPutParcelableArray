using Android.App;
using Android.Widget;
using Android.OS;
using ParcableTest.Dialogs;
using System.Collections.Generic;

namespace ParcableTest
{
    [Activity(Label = "ParcableTest", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        private SimpleSelectorItem[] _empList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it


            _empList = new SimpleSelectorItem[] {
                new SimpleSelectorItem(1, "one"),
                new SimpleSelectorItem(2, "two"),
                new SimpleSelectorItem(2, "three")
               };

            Button button = FindViewById<Button>(Resource.Id.myButton);
            button.Click += delegate
                {
                    var searchableListDialog = SearchableListDialog.newInstance(_empList);
                    searchableListDialog.Show(FragmentManager, "");
                };
        }
    }
}

