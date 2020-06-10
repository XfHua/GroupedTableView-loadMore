using System;
using CoreGraphics;
using System.IO;
using System.Collections.Generic;
using UIKit;
using Foundation;
using System.Linq;

namespace BasicTable {
	public class HomeScreen : UIViewController {
		UITableView table;

		public event EventHandler refresh;

		public HomeScreen ()
		{
			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			table = new UITableView (View.Bounds, UITableViewStyle.Grouped);
			table.AutoresizingMask = UIViewAutoresizing.All;
			CreateTableItems();
			Add (table);

            this.refresh += HomeScreen_refresh; 

			UIRefreshControl control = new UIRefreshControl();
			control.AttributedTitle = new NSAttributedString("pull to refresh");
			control.AddTarget(refresh, UIControlEvent.ValueChanged);
			table.AddSubview(control);
		}

        private void HomeScreen_refresh(object sender, EventArgs e)
        {
			Console.WriteLine("HomeScreen_refresh");
        }

        protected void CreateTableItems ()
		{
			List<TableItem> veges = new List<TableItem>();
	
			// Credit for test data to 
			// http://en.wikipedia.org/wiki/List_of_culinary_vegetables
			var lines = File.ReadLines("VegeData2.txt");
			foreach (var line in lines) {
				var vege = line.Split(',');
				veges.Add (new TableItem(vege[1]) {SubHeading=vege[0]} );
			}

			table.Source = new TableSource(veges, this);
			table.Delegate = new myDelegate(veges);
		}
	}

	public class myDelegate : UITableViewDelegate {

		Dictionary<string, List<TableItem>> indexedTableItems;
		string[] keys;

		public myDelegate(List<TableItem> items) {

			indexedTableItems = new Dictionary<string, List<TableItem>>();
			foreach (var t in items)
			{
				if (indexedTableItems.ContainsKey(t.SubHeading))
				{
					indexedTableItems[t.SubHeading].Add(t);
				}
				else
				{
					indexedTableItems.Add(t.SubHeading, new List<TableItem>() { t });
				}
			}
			keys = indexedTableItems.Keys.ToArray();
		}

		public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
			List<TableItem> tabItems = indexedTableItems[keys[indexPath.Section]];

			if (indexPath.Section == keys.Length-1 && indexPath.Row == tabItems.Count-1)
            {
				loadmoreData();
            }
        }

		public void loadmoreData() {

			Console.WriteLine("loadmoreData");
		}
    }
}