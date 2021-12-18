namespace CourseWork3.Patterns
{
    interface ICommand<T> where T: GameObjects.ControlledObject<T>
    {
        abstract public void Invoke(T gameObject);
    }
}
