using FakeItEasy;

using NUnit.Framework;

using Ovh.Api;
using Ovh.Api.Testing;
using Ovh.Test.Models;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ovh.Test;

[TestFixture]
public class PutRequests
{
    private static long currentClientTimestamp = 1566485765;
    private static long currentServerTimestamp = 1566485767;
    private static DateTimeOffset currentDateTime = DateTimeOffset.FromUnixTimeSeconds(currentClientTimestamp);
    private static ITimeProvider timeProvider = A.Fake<ITimeProvider>();

    public PutRequests()
    {
        A.CallTo(() => timeProvider.UtcNow).Returns(currentDateTime);
    }

    public static void MockAuthTimeCallWithFakeItEasy(FakeHttpMessageHandler fake)
    {
        A.CallTo(() =>
            fake.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/auth/time"))))
            .Returns(Responses.Get.time_message);
    }

    [Test]
    public async Task PUT_with_raw_string_data_and_string_result()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/contact"))))
            .Returns(Responses.Put.me_contact_message);

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PutStringAsync("/me/contact", "Fake content");
        Assert.That(Responses.Put.me_contact_content, Is.EqualTo(result));

        var contactCall = Fake.GetCalls(testHandler).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/me/contact")).First();

        var requestMessage = contactCall.GetArgument<HttpRequestMessage>("request");
        var headers = requestMessage.Headers;
        Assert.Multiple(() =>
        {
            Assert.That(HttpMethod.Put, Is.EqualTo(requestMessage.Method));
            Assert.That(Constants.APPLICATION_KEY, Is.EqualTo(headers.GetValues(Client.OVH_APP_HEADER).First()));
            Assert.That(Constants.CONSUMER_KEY, Is.EqualTo(headers.GetValues(Client.OVH_CONSUMER_HEADER).First()));
            Assert.That(currentServerTimestamp.ToString(), Is.EqualTo(headers.GetValues(Client.OVH_TIME_HEADER).First()));
            Assert.That("$1$5e81842c0f0c806fd703de084d80192a59bc0f8a", Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
        });
    }

    [Test]
    public async Task PUT_with_string_to_be_serialized_data_and_string_result()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/contact"))))
            .Returns(Responses.Put.me_contact_message);

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PutAsync("/me/contact", "Fake content");
        Assert.That(Responses.Put.me_contact_content, Is.EqualTo(result));

        var contactCall = Fake.GetCalls(testHandler).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/me/contact")).First();

        var requestMessage = contactCall.GetArgument<HttpRequestMessage>("request");
        var headers = requestMessage.Headers;
        Assert.Multiple(() =>
        {
            Assert.That(HttpMethod.Put, Is.EqualTo(requestMessage.Method));
            Assert.That(Constants.APPLICATION_KEY, Is.EqualTo(headers.GetValues(Client.OVH_APP_HEADER).First()));
            Assert.That(Constants.CONSUMER_KEY, Is.EqualTo(headers.GetValues(Client.OVH_CONSUMER_HEADER).First()));
            Assert.That(currentServerTimestamp.ToString(), Is.EqualTo(headers.GetValues(Client.OVH_TIME_HEADER).First()));
            Assert.That("$1$ec5195342ad1c81073c2eb3f3d83dd20942c4408", Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
        });
    }

    [Test]
    public async Task PUT_with_raw_string_data_and_T_result()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/contact"))))
            .Returns(Responses.Put.me_contact_message);

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PutAsync<Contact>("/me/contact", "Fake content");
        Assert.That("00000", Is.EqualTo(result.address.zip));
    }

    [Test]
    public async Task PUT_with_no_data_and_string_result()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/contact"))))
            .Returns(Responses.Put.me_contact_message);

        var patch = new { address = new { line1 = "Hey there" } };

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PutAsync("/me/contact");
        Assert.That(Responses.Put.me_contact_content, Is.EqualTo(result));

        var contactCall = Fake.GetCalls(testHandler).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/me/contact")).First();

        var requestMessage = contactCall.GetArgument<HttpRequestMessage>("request");
        var headers = requestMessage.Headers;
        Assert.That("$1$5595b180f954de130f8da7a5a4b55adc3d27556f", Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
    }

    [Test]
    public async Task PUT_with_no_data_and_T_result()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/contact"))))
            .Returns(Responses.Put.me_contact_message);

        var patch = new { address = new { line1 = "Hey there" } };

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PutAsync<Contact>("/me/contact");
        Assert.That("00000", Is.EqualTo(result.address.zip));

        var contactCall = Fake.GetCalls(testHandler).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/me/contact")).First();

        var requestMessage = contactCall.GetArgument<HttpRequestMessage>("request");
        var headers = requestMessage.Headers;
        Assert.That("$1$5595b180f954de130f8da7a5a4b55adc3d27556f", Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
    }

    [Test]
    public async Task PUT_with_T_data_and_T_result()
    {
        var testHandler = A.Fake<FakeHttpMessageHandler>(a => a.CallsBaseMethods());
        MockAuthTimeCallWithFakeItEasy(testHandler);
        A.CallTo(() =>
            testHandler.Send(A<HttpRequestMessage>.That.Matches(
                r => r.RequestUri.ToString().Contains("/me/contact"))))
            .Returns(Responses.Put.me_contact_message);

        var patch = new { address = new { line1 = "Hey there" } };

        var c = ClientFactory.GetClient(testHandler).AsTestable(timeProvider);
        var result = await c.PutAsync<Contact>("/me/contact", patch);
        Assert.That("00000", Is.EqualTo(result.address.zip));

        var contactCall = Fake.GetCalls(testHandler).Where(call =>
            call.Method.Name == "Send" &&
            call.GetArgument<HttpRequestMessage>("request").RequestUri.ToString().Contains("/me/contact")).First();

        var requestMessage = contactCall.GetArgument<HttpRequestMessage>("request");
        var headers = requestMessage.Headers;
        Assert.That("$1$747cdaf92e412ea434a387e6ff7b20150ee1172f", Is.EqualTo(headers.GetValues(Client.OVH_SIGNATURE_HEADER).First()));
    }
}