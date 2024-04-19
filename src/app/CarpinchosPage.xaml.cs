using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System.ComponentModel;
using System.Net;

namespace Carpinchazure;

public partial class CarpinchosPage : ContentPage
{
    public List<string> Carpinchos { get; set; } = new List<string>();
    public string ContainerName { get; set; }
    public bool IsPrivate { get; set; }
    public CarpinchosPage(bool isPrivate = false)
	{
		InitializeComponent();
        var containerName = "carpincho-images";
        IsPrivate = isPrivate;
        if (isPrivate)
        {
            containerName = $"{containerName}-premium";
        }
        ContainerName = containerName;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        IsBusy.IsRunning = true;
        await PopulateCarpinchos();
        CarpinchosList.ItemsSource = Carpinchos;
        IsBusy.IsRunning = false;
    }

    private async Task PopulateCarpinchos()
    {
        var connectionString = "<CONNECTION_STRING>";
        var client = new BlobServiceClient(connectionString);
        var container = client.GetBlobContainerClient(ContainerName);
        var containerUrl = container.Uri.ToString();
        try
        {
            // Call the listing operation and return pages of the specified size.
            var resultSegment = container.GetBlobsAsync().AsPages();

            // Enumerate the blobs returned for each page.
            await foreach (Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    var carpinchoUrl = $"{containerUrl}/{blobItem.Name}";
                    if (IsPrivate)
                    {
                        var sasUri = container.GenerateSasUri(BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
                        carpinchoUrl = $"{carpinchoUrl}{sasUri.Query}";
                    }
                    Carpinchos.Add(carpinchoUrl);
                }
            }
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}