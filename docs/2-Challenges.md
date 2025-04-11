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

### Variable consolidation

There are a few variables that one could in theory pass from TF outputs to the kustomize deployment variables.
For example, characters / items table right now are hardcoded in C#, but they could be passed from the
TF output to avoid inconsistencies.

### Table initialization via terraform only

I chose to initialize tables and seed via C# code due to ease of use when developing locally, however,
this is redundant with the initialization / seeding in terraform.

In a real environment, I would not have C# initialization and let all that happen only on terraform.
Maybe separate the C# initialization so that it only happens on local builds.

### EC2 instead of EKS

My experience with deployment containers has always been via kubernetes, so EC2 was completely new to me.
I had to learn a few things and change my mindset on how to deploy services, but the tradeoff seemed
worth it, given the simplicity of autoscaling groups and deployment without having to use an additional tool (kustomize).

### Userdata to setup docker and debugging EC2 instance

It was a pain to try and figure out
why my metadata service instance was not responding to health checks.
The apt-get commands were not working, and I had to enable ssh
in order to read the logs as to why that was happening.
Without being used to all the aws CLI commands, this was
a tedious process.

### Github Workflows ###

Giving the right policies to github was not trivial. It took a few attempts
plus figuring out the right flow so that I could get the variables such as
region and ecr repo passed around.
Additionally, the image format (linux/amd64) seemed not to match the expected one,
causing the binary not to execute.