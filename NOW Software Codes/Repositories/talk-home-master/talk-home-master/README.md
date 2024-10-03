# Talk Home Website

# Development

# Paket

The project uses `paket` to manage dependencies.

## Paket update

In order to update a NuGet package, `cd` into the .paket directory, from the shell run the following command, followed by the package name:

`paket update nuget`

## Resources

- Custom data type [http://umbraco.github.io/Belle/#/tutorials/manifest](here)

- Everything about setting up Umbraco [https://our.umbraco.org/documentation/reference/config/umbracosettings/](here)

## Errors & Crashes

- `Error: ContentTypeService failed to find a content type with alias...`

**Scenario:** After deleting a content type, every page of the website throws the above error.

**Cause:** Umbraco drops a cookie after previewing a page which is not removed from the browser.

**Fix:** Delete cookies

- `Outdated content is displayed in the backoffice`

**Scenario:** The content tab disaplyed out-of-sync content

**Cause:** Can happen after deleting content types and their existing content

**Fix:** Clear Umbraco content cache with the following URL `http://YOURDOMAIN/Umbraco/dialogs/republish.aspx?xml=true`
