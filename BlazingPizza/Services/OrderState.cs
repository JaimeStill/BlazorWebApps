namespace BlazingPizza.Services;

public class OrderState
{
    public bool ShowingConfigureDialog { get; private set; }
    public Pizza ConfiguringPizza { get; private set; }
    public Order Order { get; private set; } = new Order();

    private void Reset()
    {
        ConfiguringPizza = null;
        ShowingConfigureDialog = false;
    }

    public void ShowConfigurePizzaDialog(PizzaSpecial special)
    {
        ConfiguringPizza = new()
        {
            Special = special,
            SpecialId = special.Id,
            Size = Pizza.DefaultSize,
            Toppings = new()
        };

        ShowingConfigureDialog = true;
    }

    public void CancelConfigurePizzaDialog() => Reset();

    public void ConfirmConfigurePizzaDialog()
    {
        Order.Pizzas.Add(ConfiguringPizza);
        Reset();
    }
}