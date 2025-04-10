# Challenges for Promptis Prompt Question 2

## Terraform

Writing a whole set of TF provisioning scripts for AWS (I am used to GCP) required a lot of research and some trial and error, document lookup. Re-writing etc..

Testing the whole thing was the usual terrform apply excercise :).

### Certificate creation

Certificate creation and validation also took a bit as it didnt play nicely at first.
I had to delete / recreate a couple times.
My cert was in "Pending" status for over a day, until I manually added a record to my registered
domain with the proper CNAME.

### Environment Selection

A very common practice, but does muddy the codebase a bit.
I had to add environment prefixes to my tf resources, and this prefix had to propagete
to the C# code, which also had to be used to select the dynamo db tabls to use.
Ultimately I was able to find a way to register a prefix via dynamo db context

## Possible Future Improvements

There are a few variables that one could in theory pass from TF outputs to the kustomize deployment variables.
For example, characters / items table right now are hardcoded in C#, but they could be passed from the
TF output to avoid inconsistencies.