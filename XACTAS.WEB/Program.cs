var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

#region Подключение сервисов
services.AddControllers();
services.AddLocalization(option => option.ResourcesPath = "Resources");
services.AddRazorPages();
services.AddServerSideBlazor();
#endregion

var app = builder.Build();

#region Подключение Middleware
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseRequestLocalization(GetLocalizerOptions());
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});
#endregion

app.Run();

static RequestLocalizationOptions GetLocalizerOptions()
{
    var supportedCultures = new[] { "ru-RU", "en-US" };
    return new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
}