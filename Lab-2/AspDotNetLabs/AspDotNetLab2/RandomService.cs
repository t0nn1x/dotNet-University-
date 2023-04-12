using System;

public class RandomService
{
    private readonly int _randomNumber;

    public RandomService()
    {
        _randomNumber = new Random().Next();
    }

    public int RandomNumber => _randomNumber;
}