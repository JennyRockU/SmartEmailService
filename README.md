# SmartEmailService
Web application to send emails via API

## Description
The SmartEmailService project is a simple API which accepts the email details and sends the email to its recipients by unifying different email service vendors. While this is largely a back-end project, the code in this repository also include a static page with the [API documentation](http://smartemailservice.azurewebsites.net).

## Architecture
The [SendEmailApi](../SendEmailApi) receive a POST call with the relevant email details (recipients, subject etc.) and adds this message to a queue. Later, there are two separate cloud application roles which read from this queue and try to handle the message and deliver it to its recipients. Each role uses the API of a different email provider ([SendGrid](https://sendgrid.com/docs/API_Reference/Web_API/mail.html), [MailGun](http://documentation.mailgun.com/quickstart.html#sending-messages)). If one service is down/fails to send the message, it is being returned back to the queue and then attempted again by another service.

Here is an overview of this design:

Email Message from User :arrow_down:

(sent to API)

**_SendEmailAPI (WebAPI)_** :arrow_down:
      
(sends the message to queue)

**_Cloud Message Queue_**

**_Email Sender Roles_** :arrow_heading_up:

Email message is picked up from queue and processed by one of the vendors:

- SendGrid
- MailGun

## Scalability
The fact that the different email vendors are running as separate applications in the cloud, and barely share any code (except for the [IMailSender interface](../master/MainSender/IMailSender.cs)), allows a lot of scaling flexibility.

For example, once our SendEmailService grows larger and fills up the queue with a lot of email messages which await to be processed, it is very easy to add another application to help reduce the load and read from this queue.
All that is needed is to select another email service provider (e.g.: Amazon SES, Sparkpost) and deploy a role that is reading from the same queue, that's it.

It is also possible to increase the parallelism/role instance amount so that each (or just one) provider will run in parallel and process faster messages from the queue.

This design also allows flexibility in the maintenance of each role. If for instance, if SendGrid vendor requires a code change/fix, or if it goes down, the current architecture allows deploying/shutting off SendGrid's role individually. 

## Tradeoff Decisions
While this current architecture is highly useful for scalability purposes when the application usage increases, this has limitations as well and also might not be the best choice for other types of use-cases.

- Since each email provider runs as its own standalone application, if there's a need to modify something in its main run, this would have to be done separately per each email provider role. For example, if we'd like to increase the threshold of the dequeue count after which the message will no longer be picked up by any of the roles it would require modifying each role's code separately.

- If there are changes needed in the Interface class which all the vendor roles implement, while this would require a change only in the one file, each role would need to be deployed in order for this change to take effect.

- There also additional complexity in monitoring the roles as at the moment their logs are distrbuted between different projects.

While these limitations cannot be ignored, taking all of this into account along with the app requirements, these are a small price to pay in order to allow greater flexibility in scale. Additionally, there are descent workarounds for these issues, e.g.: moving some settings to a configuration file.

## Tests
The code includes unit tests for each the main methods and each of the email sender roles.
These are simple sanity tests to run after each code change in a method.

The tests can be found here:

-[Sendgrid sender app tests](../master/SendgridSenderRole/SendgridSendeUnitTests).

-[Mailgun sender app tests](../master/MailgunSenderRole/MailgunSenderUnitTests).

Integration and worker role and API tests are still needed as well.

## Supported Features
The application currently uses only two email providers, but there is nothing that is standing in the way of adding additional vendors (the only consideration was a time constraint).

The focus of this project was on the code quality and service quality to provide the basic functionality of sending an email message. For that reason, some features were deliberately not implemented at the moment. Some of these main unsupported capabilities are:
- CC, BCC recipients
- Email attachments
- HTML message
- Attempt to unsend

## Hosting
Both the backend and client-side applications are hosted on Microsoft Azure.

## Monitoring
Errors and other useful logs managed via Application Insights, an application performance management platform.
This allows tracking errors and the application usage in an easy way, it additionally alerts by email on increased failure rates.

Each email sender role, and the SendEmailAPI report the exceptions to Application Insights telemetry.

## External Libraries
Some external libraries and SDKs were installed to help this flow running.

Libraries mostly include the designated code for using the email providers' API, such as:
- smtpapi-csharp (for SendGrid)
- restSharp (helping to create web requests in .NET)

And the SDKs are: 
- Application Insights (for application usage and error monitoring)
- Azure Storage (for the message queue)

## Technology Choices
This application built for the most part in C#, and uses all .Net technologies.
While I can work with other languages as well, on different levels, C# is the language in which I am most expiriencied.
Therefore, .Net was prioritized over other more popular backend technologies in order to avoid hurting the quality of the application code due to not having enough expirience in working with them.

The frontend application, static site, uses a little bit of JavaScript.

## Website (Frontend)
The static [website](http://smartemailservice.azurewebsites.net) ([EmailService](../master/EmailService%20-%20website) files), includes the API documentation and a form that allows to test the API by submitting a form with the email details. The form calls the SendEmailAPI.

As this is a backend project, this site has very minimal functionality and is simply intended to demonstrate the API.


##
_Love getting feedback and making my applications even better, let's get in touch!_

[Jenny R](https://www.linkedin.com/in/jennyrukman/).
