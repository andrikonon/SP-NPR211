namespace DAL.Events;

public class UserEvents
{
    // повідомляє кількість доданих користувачів
    public delegate void UserInsertItemDelegate(int count);
}