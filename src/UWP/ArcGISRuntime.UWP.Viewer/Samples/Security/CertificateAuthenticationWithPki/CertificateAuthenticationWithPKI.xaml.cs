﻿// Copyright 2019 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Offline;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.ArcGISServices;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Esri.ArcGISRuntime.Portal;
using Windows.UI.Popups;
using Esri.ArcGISRuntime.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace ArcGISRuntime.UWP.Samples.CertificateAuthenticationWithPKI
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Certificate authentication with PKI",
        "Security",
        "Access secured portals using a certificate.",
        "")]
    [ArcGISRuntime.Samples.Shared.Attributes.OfflineData()]
    public partial class CertificateAuthenticationWithPKI
    {
        private const string ServerUrl = "https://portallxpkids.esri.com/gis/";

        public CertificateAuthenticationWithPKI()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
        }

        public async Task<Credential> CreateCertCredentialAsync(CredentialRequestInfo info)
        {
            // Handle challenges for a secured resource by prompting for a client certificate
            Credential credential = null;

            try
            {
                // Use the X509Store to get a collection of available certificates
                var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                var certificates = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, true);

                // Make sure the user chose one
                if (true) //(selection.Count > 0)
                {
                    // Create a new CertificateCredential using the chosen certificate
                    credential = new Esri.ArcGISRuntime.Security.CertificateCredential(certificates[0])
                    {
                        ServiceUri = new Uri(ServerUrl)
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
            }

            // Return the CertificateCredential for the secured portal
            return credential;
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                ArcGISPortal portal = await ArcGISPortal.CreateAsync(new Uri(ServerUrl), null, await CreateCertCredentialAsync(null), new System.Threading.CancellationToken());
                await new MessageDialog(portal.User.FullName).ShowAsync();
            }
            catch (Exception ex)
            {
            }
        }
    }
}