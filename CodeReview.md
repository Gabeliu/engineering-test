1. Invalid using directive

using System.Collegctions.Generic;

The namespace is misspelled and should be System.Collections.Generic.
This prevents compilation and should be corrected immediately.

2. Type mismatch and age threshold logic issues

private static readonly DateTimeOffset Under16 = DateTimeOffset.UtcNow.AddYears(-15);
public People(string name) : this(name, Under16.Date) { }

Under16 indicates age is "under 16", but the code uses AddYears(-15), which is 1 year off.
Using UtcNow in a static field freezes the date forever.
Since DateTimeOffset contains both date and timezone offset, passing Under16.Date, a DateTime without offset loses the offset information.

3. Incorrect usage of Random()

if (random.Next(0, 1) == 0) {
                        name = "Bob";
                    }
                    else {
                        name = "Betty";
                    }

The statement Next(0, 1) will always return 0. This means "Betty" will never be selected.
It should be changed to random.Next(0, 2)
Also, calling new Random() inside a loop will result in repeated values. 
var random = new Random(); should be called outside of for loop.

4. Incorrect age calculation 

random.Next(18, 85) * 356

A year is approximated as 365 days, but the code uses 356.
Converting years to days is unreliable because of leap years.
The correct pattern would be subtracting whole years using AddYears(-age) instead of subtracting days.

5. Exception handling removes root causes

catch (Exception e)
                {
                    // Dont think this should ever happen
                    throw new Exception("Something failed in user creation");
                }

Exception e was caught but never used. Then the code throw a new Exception but never explained where the error happened, instead it is replaced with "something failed".
Correct verison would be:

catch (Exception e)
                {
                    throw;
                }

6. GetBobs uses incorrect logic for age comparison

x.DOB >= DateTime.Now.Subtract(new TimeSpan(30 * 356, 0, 0, 0)) : _people.Where(x => x.Name == "Bob");

This finds Bobs younger than 30 instead of older.
Also uses 356 instead of 365.
This uses DateTime.Now while other parts of the code uses UtcNow which is inconsistent. 
It also compares DateTimeOffset with DateTime which may introduce incorrect outcomes. 
It would be consistent and easier to use:

DateTimeOffset.UtcNow.AddYears(-30)

7. GetMarried contains multiple logical issues

  public string GetMarried(People p, string lastName)
        {
            if (lastName.Contains("test"))
                return p.Name;
            if ((p.Name.Length + lastName).Length > 255)
            {
                (p.Name + " " + lastName).Substring(0, 255);
            }

            return p.Name + " " + lastName;
        }

p.Name.Length is an int, and lastName is a string, so (p.Name.Length + lastName).Length is not a meaningful way to check the full name length.
To validate the length correctly, we should first build the full name string and then check fullName.Length, for example:
string fullName = p.Name + " " + lastName; then if (fullName.Length > 255)
This statement here: (p.Name + " " + lastName).Substring(0, 255); fullname is not modified here, it just returns a new string.
There are no null-checks for p, p.Name, and lastName;
lastName.Contains("test") will throw if lastName is null, and it is case-sensitive.

Correct version: 

public string GetMarried(People p, string lastName)
{
    if (p == null)
        throw new ArgumentNullException(nameof(p));

    if (string.IsNullOrWhiteSpace(p.Name))
        throw new ArgumentException("Person must have a valid name.", nameof(p));

    if (string.IsNullOrWhiteSpace(lastName))
        throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));

    if (lastName.Contains("test", StringComparison.OrdinalIgnoreCase))
        return p.Name;

    // Build the full name once
    string fullName = p.Name + " " + lastName;

    const int maxLength = 255;
    if (fullName.Length > maxLength)
        fullName = fullName.Substring(0, maxLength);

    return fullName;
}

8. Naming and style inconsistencies

Class People represents a single person and should be named Person.
XML comment of /// <param name="j"></param> should be "i" since the actual method parameter name uses int i.
Comments contain typos: "dandon Name" should be "random name", and "Dont" should be "Don't"
Method names could be a more descriptive, example: GetPeople(int i) could be improved to GeneratePeople(int count).
It is best practice to avoid magic numbers like 356. Such values should be replaced with well-named constants to make the code self-documenting, easier to maintain, and less error-prone.

