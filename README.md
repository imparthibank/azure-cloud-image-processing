# Azure cloud image processing

This project is mainly focus how to upload and view the images by azure cloud platform using storage account.

The uploaded images are stored into azure blob container using block blob storage type and once an image uploaded into images blob container, it'll show in the UI.

The image Uri will be having temporary shared access key for the authorization, that will be expired 15 minutes of the image uri generation from blob storage references.
