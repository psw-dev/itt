using Microsoft.Extensions.Configuration;
using PSW.ITT.Data;
using PSW.ITT.Data.Sql.UnitOfWork;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.IServices;
using PSW.ITT.Service.Strategies;
using PSW.RabbitMq;
using PSW.RabbitMq.ServiceCommand;
using System.Text.Json;

namespace PSW.ITT.Api.RabbitMq
{
    public class ITTRabbitMqListener : RabbitMqListener
    {
        //TODO: Inject
        private IITTService _service { get; set; }



        public ITTRabbitMqListener(IITTService service, IUnitOfWork uow, ISHRDUnitOfWork shrdUow, IConfiguration configuration)
        : base(configuration)
        {
            // uow.BeginTransaction();
            _service = service;
            _service.UnitOfWork = uow;
            _service.SHRDUnitOfWork = shrdUow;
            _service.StrategyFactory = new StrategyFactory(uow, shrdUow);
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