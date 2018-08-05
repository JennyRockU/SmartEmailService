# SmartEmailService
Web application to send emails via API

## Description
The SmartEmailService project is a simple API which accepts the email details and sends the email, by unifiying different email service vendors. While this is largely a back-end project, the code in this repository also include a static page with the [API documentation](http://smartemailservice.azurewebsites.net).

## Architecture
The [SendEmailApi](../SendEmailApi) recieves POST calls with the relevant email details (recipients, subject etc.) and adds this message to a queue. Later, there are two seperate cloud application roles which read from this queue and try to handle the message. Each role uses the API of a different email provider ([SendGrid](https://sendgrid.com/docs/API_Reference/Web_API/mail.html), [MailGun](http://documentation.mailgun.com/quickstart.html#sending-messages)). If one service is down/fails to send the message, it is being returned back to the queue and then attempted again by another service.

Here is an overview of this desgin:

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
The fact that the different email vendors are running as seperate applications in the cloud, and barely share any code (except for the [IMailSender interface](../master/MainSender/IMailSender.cs)), allows a lot of scaling flexibilty.

For example, once our SendEmailService grows larger and fills up the queue with a lot of email messages which await to be processed, it is very easy to add another application to help reduce the load and read from this queue.
All that is needed is to select another email service provider (e.g.: Amazon SES, Sparkpost) and deploy a role that is reading from the same queue, that's it.

It is also possible to increase the parallelism/role instance amount so that each (or just one) provider will run in parallel and process messages from the queue faster.

This design also, allows flexibity in the maintainance of each role. If for instance, SendGrid vendor requires a code change/fix, or if it goes down, the current architecture allows deploying/shutting off SendGrid's role individually. 

## Tradeoff Decsicions
While this current architecture is highly useful for scalability purposes, as the application usages increase, it has limitations as well and also might not be advisable for other types of use-cases.

- Since each email provider runs as its own application, if there's need to modify something in its main run, this would have to be done seperately per each email provider role. For example, if we'd like to increase the threshold of the deqeue count after which the message will no longer be picked up by the role it would require to access each role's code seperately.

- If there are needed changes in the Interface class that all the vendor roles implement, while this would require a change only in the one file, each role would need to be deployed in order for this change to take effect.

- There also additional complexity in monitoring the roles.

While these limitations cannot be ignored, taking all of this into account along with the app requirements, these are small prices to pay in order to allow greater flexibility in scale. Additionally, there are descent workarounds for these issues, e.g.: moving some settings to a configuration file.

## Supported Features
The application currently uses only two email providers, but there is nothing that is standing in the way of adding additional vendors (the only consideration was a time constraint).

The focus of this project the code quality and service quality of providing the basic funcationality of sending an email message. For that reason, some features were delibirately not implemented at the moment. Some of these main unsupported capabilities are:
- CC, BCC recipients
- Email attachments
- HTML message
- Attempt to unsend

## Hosting
Both the backend and client-side applications are hosted on Microsoft Azure.

## Monitoring
Errors and other useful logs managed via Application Insights, a application performance management platform.
This allows to track errors and the application usage in an easy way, it additionally alerts by email on increased failure rates.

Each email sender role, and the SendEmailAPI report the exceptions to Application Insights telemtry.

## External Libraries
Some external libraries and SDKs were installed to help this flow running.

Libraries mostly include the designated code for using the email providers' API, such as:
- smtpapi-csharp (for SendGrid)
- restSharp (helping to create web requests in .NET)

And SDKs: 
- Application Insights (for application usage and error monitoring)
- Azure Storage (for the message queue)

## Technology Choices
This application is for the most part built in C#, and uses all .Net technologies.
While can work with other languages as well, on different levels, C# is the language in which I am most expiriencied.
Therefore, .Net was prioritized over other more popluar backend technologies in order to avoid hurting the quality of the applocation code due to lack of expirence.

The frontend application, static site, uses a little bit of Javascript.

## Website (Frontend)
The static [website](http://smartemailservice.azurewebsites.net) ([EmailService](../master/EmailService%20-%20website) files), includes the API documentation and a form that allows to test the API by submitting a form with the email details. The form calls the SendEmailAPI.

As this is a backend project, this site has very minimal functionality and is simply intended to demonstrate the API.


##
_Love getting feedback and making my applications even better, let's get in touch!_

[Jenny R](https://www.linkedin.com/in/jennyrukman/).
