﻿using NSpec;

class given_the_sequence_continues_with_2 : given_the_sequence_starts_with_1
{
    void before_each()
    {
        sequence += "2";
    }
    void given_the_sequence_continues_with_3()
    {
        before = () => sequence += "3";

        //the befores run in the order you would expect
        it["sequence should be \"123\""] =
            () => sequence.should_be("123");
    }
}

class given_the_sequence_starts_with_1 : nspec
{
    void before_each()
    {
        sequence = "1";
    }
    protected string sequence;
}

public static class given_the_sequence_continues_with_2_output
{
    public static string Output = @"
given the sequence starts with 1
  given the sequence continues with 2
    given the sequence continues with 3
      sequence should be ""123""

1 Examples, 0 Failed, 0 Pending
";
    public static int ExitCode = 0;
}
