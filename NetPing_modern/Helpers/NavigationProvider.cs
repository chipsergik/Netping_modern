using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using NetPing_modern.Models;
using NetPing_modern.Resources.Views.Catalog;

namespace NetPing_modern.Helpers
{
    public class NavigationProvider
    {
        public static class Groups
        {
            public static readonly string Monitoring = "Monitoring-servernoj";
            public static readonly string Power = "Upravljaemye-rozetki-ip-pdu";
            public static readonly string Switch = "Kommutatory-ethernet";
        }

        public static List<SectionModel> GetAllSections()
        {
            var sections = new List<SectionModel>();

                sections.Add(new SectionModel
                                {
                                    Description = Index.Sec_monitoring_descr,
                                    ImageUrl = "../../Content/Images/present_left_img1.png",
                                    Title = Index.Sec_monitoring,
                                    Url = Groups.Monitoring,
                                    Sections = new List<SectionModel>
                                                {
                                                    new SectionModel
                                                    {
                                                        Title = Index.Sec_sub_sensors,
                                                        Url = "sensors"
                                                    },
                                                    new SectionModel
                                                    {
                                                        Title = Index.Sec_sub_access,
                                                        Url = "accessories",
                                                    },
                                                    new SectionModel
                                                    {
                                                        Title = Index.Sec_sub_solutions,
                                                        Url = "Dlja-servernyh-komnat-i-6kafov",
                                                    }
                                                }
                                });

                sections.Add(new SectionModel
                                {
                                    Description = Index.Sec_power_descr,
                                    ImageUrl = "../../Content/Images/present_left_img2.png",
                                    Title = Index.Sec_power,
                                    Url = Groups.Power,
                                    Sections = new List<SectionModel>
                                                {
                                                    new SectionModel
                                                    {
                                                        Title = Index.Sec_sub_sensors,
                                                        Url = "sensors"
                                                    },
                                                    new SectionModel
                                                    {
                                                        Title = Index.Sec_sub_access,
                                                        Url = "accessories",
                                                    },
                                                    new SectionModel
                                                    {
                                                        Title = Index.Sec_sub_solutions,
                                                        Url = "Udaljonnoe-upravlenie-jelektropitaniem",
                                                    }
                                                }
                                });

                sections.Add(new SectionModel
                {
                    Description = Index.Sec_switch_descr,
                    ImageUrl = "../../Content/Images/present_left_img3.png",
                    Title = Index.Sec_switch,
                    Url = Groups.Switch,
                    Sections = new List<SectionModel>
                                                {
                                                    new SectionModel
                                                    {
                                                        Title = Index.Sec_sub_access,
                                                        Url = "accessories",
                                                    },
                                                    new SectionModel
                                                    {
                                                        Title = Index.Sec_sub_solutions,
                                                        Url = "Re6enija-na-osnove-POE",
                                                    }
                                                }
                });
            return sections;
        }
    }
}