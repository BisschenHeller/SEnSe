using System;

public class IllegalSensorSubscriptionException : Exception
{
    public IllegalSensorSubscriptionException(string attemptedType, string myType)
        : base(GetVerb(attemptedType, myType))
    {
        
    }

    private static string GetVerb(string attemptedType, string myType)
    {
        return string.Format("Cannot subscribe a <{0}> action to a <{1}> Sensor.", attemptedType, myType);
    }
}

public class IllegalSensorExpositionException : Exception
{
    public IllegalSensorExpositionException(string attemptedType, string myType)
        : base(GetVerb(attemptedType, myType))
    {

    }

    private static string GetVerb(string attemptedType, string myType)
    {
        return string.Format("Cannot expose a <{0}> observable on a <{1}> Sensor.", attemptedType, myType);
    }
}