using System.Collections.Generic;
using System.Windows.Controls;

namespace PGI.Views.Stocks
{
    public partial class SuppliersView : UserControl
    {
        public SuppliersView()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            var suppliers = new List<Supplier>
            {
                new Supplier { Code = "NS-001", Nom = "Nordic Supplies", Email = "contact@nordicsupplies.com", Delai = "7", Escompte = "5%" },
                new Supplier { Code = "AC-002", Nom = "Adventure Co.", Email = "info@adventureco.com", Delai = "10", Escompte = "3%" },
                new Supplier { Code = "MG-003", Nom = "Mountain Gear", Email = "sales@mountaingear.com", Delai = "5", Escompte = "7%" },
            };

            SuppliersDataGrid.ItemsSource = suppliers;
        }
    }

    public class Supplier
    {
        public string Code { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Delai { get; set; }
        public string Escompte { get; set; }
    }
}

