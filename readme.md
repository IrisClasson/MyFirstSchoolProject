**Please use the Base branch for branching - DO NOT use the Original branch for edits or branching**

For a long time I’ve wanted to do an OSS project, I’ve gotten to know so many fantastic people and I would love to learn and work together with them, with you. I just didn’t know what to do. What can I contribute? Well, by ugly ass code from my very first application- and a brilliant idea. That’s what I have.

I want us, together to code review and refactor the application and make it available to other developers to learn, mainly students and junior developers, to see how we that know this (more or less ha ha) would go about cleaning up this code. 
I know it might end up being hard to read from the changes, but I will document the changes with ‘before and after’ explaining based on the commit messages what was done and why. Since the application is small, but still has many basic common features such as CRUD operations, working with images, saving serialized data to a file, events (only one I believe), UX, working with calendars and so on.

I tried to make some rules that would help us better put this together, please let me know if you have some better ideas/additional ideas. 

**‘Rules’**

* Keep third party libs to a minimum, use the smallest ones with the least dependencies
-    This makes the code easier to understand for students
For complete refactoring (for example to a different platform) please do a branch
-	This is so we can compare different ideas, and avoid conflicting ideas
Frequent small commits – but nothing that breaks the build!
-	You know why :)
Provide a description when you commit with what and why
-	This is so others can learn from this
Use Github Issues for codereview – each ‘comment’ can be an issue
-	I’m open to other ideas here :)

**The app should/user should:**

* Be able to save, edit and delete customers
-	Customer images
-	Create a booking calendar
-	Save, Edit and delete appointments
-	Track customer conversations/ notes
-	Sava data locally by serializing
-	No third party libraries allowed
-	Use an event
-	User friendly
-	Good design (UI)

This application was a mini-CRM (Customer Relationship Management system) made in Windows Forms (we were not allowed to choose ourselves, and didn’t know anything else existed). On my team we were three developers, two of us had no programming experience from before, and one had done a year or two CS at uni. This was about 1 month into the education, and was our very first application aside from one console app and we had 7 working days to do this.

