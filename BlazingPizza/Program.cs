using BlazingPizza.Data;
using BlazingPizza.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddSqlite<PizzaStoreContext>("Data Source=pizza.db");
builder.Services.AddScoped<OrderState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

IServiceScopeFactory factory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = factory.CreateScope();
PizzaStoreContext db = scope.ServiceProvider.GetRequiredService<PizzaStoreContext>();

if (await db.Database.EnsureCreatedAsync())
    await SeedData.Initialize(db);

app.Run();