using Microsoft.Extensions.Configuration;
using psw.itt.data;
using psw.itt.data.sql.UnitOfWork;
using psw.itt.service.Command;
using psw.itt.service;
using psw.itt.service.Strategies;
using PSW.RabbitMq;
using PSW.RabbitMq.ServiceCommand;
using System.Text.Json;

namespace psw.itt.api.RabbitMq
{
    public class ITTRabbitMqListener : RabbitMqListener
    {
        //TODO: Inject
        private IITTService _service { get; set; }



        public ITTRabbitMqListener(IITTService service, IUnitOfWork uow, IConfiguration configuration)
        : base(configuration)
        {
            // uow.BeginTransaction();
            _service = service;
            _service.UnitOfWork = uow;
            _service.StrategyFactory = new StrategyFactory(uow);
        }

        public override void ProcessMessage(ServiceRequest request, IConfiguration configuration, IEventBus eventBus, string correlationId, string replyTo)
        {
            _service.UnitOfWork = new UnitOfWork(configuration, eventBus);

            //Adding ServiceRequest data to CommandRequest and
            // invoking service method
            var commandReply = _service.invokeMethod(new CommandRequest()
            {
                methodId = request.methodId,
                data = JsonDocument.Parse(request.data).RootElement
            });

            //Checking if there is a need to reply back after processing request
            if (string.IsNullOrEmpty(correlationId))
            {
                return;
            }

            //Adding CommandReply data to ServiceReply and
            //publishing reply to provided queue name in replyTo
            eventBus.PublishReply(new ServiceReply()
            {
                code = commandReply.code,
                data = commandReply.data.GetRawText(),
                shortDescription = commandReply.shortDescription,
                fullDescription = commandReply.fullDescription,
                message = commandReply.message,
                exception = commandReply.exception
            }, replyTo, correlationId);
        }
    }
}